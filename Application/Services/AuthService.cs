using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
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
        //private readonly IMemberRepository memberRepository;
        private readonly UserManager<Member> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration configuration;

        public AuthService(UserManager<Member> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.configuration = configuration;
        }

        public string createToken(Member member,IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email , member.Email),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["tokensecret"]));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken
            (
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials : creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        public async Task<string> Login(LoginMemberDto loginMemberDto)
        {
            Member user = await _userManager.FindByEmailAsync(loginMemberDto.Email);
            if (user == null) return null;
            var result = await _userManager.CheckPasswordAsync(user, loginMemberDto.Password);
            if (result == false) return null;
            var userRoles = await _userManager.GetRolesAsync(user);
            return createToken(user,userRoles);
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
    }
}
