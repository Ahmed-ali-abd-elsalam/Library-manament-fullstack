using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IAuthService
    {
        Task<LoginResponseDto> refresh(string userEmail, string refreshToken,string source);
        Task<LoginResponseDto> Login(LoginMemberDto loginMemberDto,string source);
        Task<MemberResponseDto> Signup(RegisterMemberDto registerMemberDto);
        public Task<bool> logOutAsync(string email, string token);

    }
}
