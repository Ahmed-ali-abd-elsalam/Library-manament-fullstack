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
        private readonly IMemberRepository memberRepository;
        private readonly IConfiguration configuration;

        public AuthService(IMemberRepository memberRepository, IConfiguration confguration)
        {
            this.memberRepository = memberRepository;
            this.configuration = confguration;
        }

        public string createToken(Member member)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , member.Name),
                new Claim(ClaimTypes.Email , member.Email),
                new Claim(ClaimTypes.Role , member.Role)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")));
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
            if (!await memberRepository.CheckExistsAsync(loginMemberDto.Email)) return null;
            Member member = await memberRepository.GetMemberAsync(loginMemberDto.Email);
            if (new PasswordHasher<Member>().VerifyHashedPassword(member, member.HashedPassword, loginMemberDto.Password)
                == PasswordVerificationResult.Failed) return null;
            return createToken(member);
        }

        public async Task<MemberResponseDto> Signup(RegisterMemberDto registerMemberDto)
        {
            if (await memberRepository.CheckExistsAsync(registerMemberDto.Email)) return null;
            Member member = registerMemberDto.RegisterDtoToMember();
            member.HashedPassword = new PasswordHasher<Member>().HashPassword(member, registerMemberDto.Password);
            memberRepository.AddMemberAsync(member);
            return member.ToMemberResponseDto();

        }
    }
}
