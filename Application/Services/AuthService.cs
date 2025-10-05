using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Domain.Entities;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Member> _userManager;
        private readonly IMemberService memberService;
        private readonly IUserTokenService tokenService;
        private readonly IDistributedCache cache;
        private readonly IFluentEmail fluentEmail;
        private readonly IConfirmEmailTokenService ConfirmEmailService;
        private readonly LinkFactory linkFactory;

        public AuthService(UserManager<Member> userManager,
            IUserTokenService tokenService,
            IDistributedCache cache,
            IFluentEmail fluentEmail,
            IConfirmEmailTokenService ConfirmEmailService,
            LinkFactory linkFactory,
            IMemberService memberService)
        {
            _userManager = userManager;
            this.tokenService = tokenService;
            this.cache = cache;
            this.fluentEmail = fluentEmail;
            this.ConfirmEmailService = ConfirmEmailService;
            this.linkFactory = linkFactory;
            this.memberService = memberService;
        }

        public async Task<LoginResponseDto> Login(LoginMemberDto loginMemberDto, string source, CancellationToken cancellationToken)
        {
            Member user = await _userManager.FindByEmailAsync(loginMemberDto.Email);
            if (user == null) return null;
            bool result = await _userManager.CheckPasswordAsync(user, loginMemberDto.Password);
            if (result == false || !user.EmailConfirmed) return null;
            var userRoles = await _userManager.GetRolesAsync(user);
            string key = $"{loginMemberDto.Email}-{source}";
            string ResponseToken =  await cache.GetStringAsync(key, cancellationToken);
            if (ResponseToken is not null){
                return new LoginResponseDto
                {
                    Email = loginMemberDto.Email,
                    Response_Token = ResponseToken,
                    Refresh_token = await tokenService.createTokenAsync(user, userRoles, "Refresh Token", source)
                };
            }
            ResponseToken = await tokenService.createTokenAsync(user, userRoles, "Response Token", source);
            await cache.SetStringAsync(
                key,
                ResponseToken,
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)),
                cancellationToken);
            return new LoginResponseDto
            {
                Email = loginMemberDto.Email,
                Response_Token = ResponseToken,
                Refresh_token = await tokenService.createTokenAsync(user, userRoles, "Refresh Token", source)
            };

            }
        public async Task<LoginResponseDto> refresh(string userEmail,string RefreshToken,string source,CancellationToken cancellationToken)
        {
            Member user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return null;
            var userRoles = await _userManager.GetRolesAsync(user);
            string key = $"{userEmail}-{source}";
            string ResponseToken = await cache.GetStringAsync(key, cancellationToken);
            if (ResponseToken is not null)
            {
                return new LoginResponseDto
                {
                    Email = userEmail,
                    Response_Token = ResponseToken,
                    Refresh_token = RefreshToken
                };
            }
            ResponseToken = await tokenService.createTokenAsync(user, userRoles, "Response Token", source);
            await cache.SetStringAsync(
                key,
                ResponseToken,
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)),
                cancellationToken);
            return new LoginResponseDto
            {
                Email = userEmail,
                Response_Token = ResponseToken,
                Refresh_token = RefreshToken
            };
        }

        public async Task<MemberResponseDto> Signup(RegisterMemberDto registerMemberDto)
        {

            Member user = await _userManager.FindByEmailAsync(registerMemberDto.Email);
            if (user != null) return null;
            Member member = registerMemberDto.RegisterDtoToMember();
            var result = await _userManager.CreateAsync(member, registerMemberDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(member, "Member");
                ConfirmEmailToken confirmationToken = await ConfirmEmailService.generateEmailTokenAsync(member.Email);                
                string link = linkFactory.generateLink("ConfirmEmail", member.Email, confirmationToken.id.ToString());
                await fluentEmail
                    .To(member.Email)
                    .Subject("Email Confirmatiom")
                    .Body($"To Validate Email <a href=\"{link}\">click here</a>", isHtml: true)
                    .SendAsync();
                return member.ToMemberResponseDto();
            }
            else
            {
                Console.WriteLine(result.Errors);
                return null;
            }
        }
        public async Task<bool> logOutAsync(string email,string source,CancellationToken cancellationToken)
        {
            Member user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            string key = $"{email}-{source}";
            await cache.RemoveAsync(key, cancellationToken);
            return true;
        }

        public async Task<bool> forgotPassword(ForgotPasswrodDTO forgotPasswrodDTO)
        {
            Member? user =await _userManager.FindByEmailAsync(forgotPasswrodDTO.Email);
            if (user is null) return false;
            if (!forgotPasswrodDTO.NewPassword.Equals(forgotPasswrodDTO.ConfirmNewPassword)) return false;
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, forgotPasswrodDTO.NewPassword);
            if (!result.Succeeded)
            {
                Console.WriteLine(result.Errors.ToList());
                return false;
            }
            return true;
        }

        public async Task SendEmail(string Email)
        {
            string link = "https://localhost:7205/api/auth";
            await fluentEmail
                .To(Email)
                .Subject("Email Confirmation")
                    .Body($"To Validate Email <a href=\"{link}\">click here</a>",isHtml:true)
                .SendAsync();
           
        }

        public async Task<bool> confirmEmail(string Email, string TokenId)
        {
            Member? user = await _userManager.FindByEmailAsync(Email);
            if (user is null) return false;
            bool validateToken = await ConfirmEmailService.ValidateEmailTokenAsync(Guid.Parse(TokenId));
            if (!validateToken) return false;
            user.EmailConfirmed = true;
            return await memberService.editMember(Email,user);            
           
        }
    }
}
