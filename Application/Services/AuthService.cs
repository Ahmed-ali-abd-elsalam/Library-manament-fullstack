using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService 
    {
        private readonly UserManager<Member> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserTokenService tokenService;
        public AuthService(UserManager<Member> userManager, RoleManager<IdentityRole> roleManager, IUserTokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.tokenService = tokenService;
        }

        public async Task<LoginResponseDto> Login(LoginMemberDto loginMemberDto,string source)
        {
            Member user = await _userManager.FindByEmailAsync(loginMemberDto.Email);
            if (user == null) return null;
            var result = await _userManager.CheckPasswordAsync(user, loginMemberDto.Password);
            if (result == false) return null;
            var userRoles = await _userManager.GetRolesAsync(user);
            string Response_Token =await tokenService.getUserTokenAsync(user.Email, source);
            if (Response_Token == null) Response_Token = tokenService.createTokenAsync(user, userRoles, "Response Token", source);
            return new LoginResponseDto {Email= loginMemberDto.Email,
                Response_Token = Response_Token,
                Refresh_token = tokenService.createTokenAsync(user,userRoles, "Refresh Token",source)
            };
        }
        public async Task<LoginResponseDto> refresh(string userEmail,string RefreshToken,string source)
        {
            Member user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return null;
            var userRoles = await _userManager.GetRolesAsync(user);
            return new LoginResponseDto
            {
                Email = userEmail,
                Response_Token = tokenService.createTokenAsync(user, userRoles, "Response Token",source),
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
        public async Task<bool> logOutAsync(string email,string token)
        {
            Member user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            UserToken userToken = await tokenService.getTokenAsync(token);
            if (userToken == null) return false;
            return await tokenService.deleteTokenAsync(userToken);
        }

    }
}
