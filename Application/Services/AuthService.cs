using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        // TODO Remove This
        private readonly IUserTokenService tokenService;
        private readonly IDistributedCache cache;
        public AuthService(UserManager<Member> userManager, RoleManager<IdentityRole> roleManager, IUserTokenService tokenService, IDistributedCache cache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.tokenService = tokenService;
            this.cache = cache;
        }

        public async Task<LoginResponseDto> Login(LoginMemberDto loginMemberDto,string source,CancellationToken cancellationToken)
        {
            Member user = await _userManager.FindByEmailAsync(loginMemberDto.Email);
            if (user == null) return null;
            bool result = await _userManager.CheckPasswordAsync(user, loginMemberDto.Password);
            if (result == false) return null;
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
            //UserToken userToken = await tokenService.getTokenAsync(token);
            //if (userToken == null) return false;
            //return await tokenService.deleteTokenAsync(userToken);
        }

    }
}
