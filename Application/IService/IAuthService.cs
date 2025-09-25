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
        Task<LoginResponseDto> refresh(string userEmail, string refreshToken);

        Task<LoginResponseDto> Login(LoginMemberDto loginMemberDto);
        Task<MemberResponseDto> Signup(RegisterMemberDto registerMemberDto);
        string createToken(Member member,IList<string> roles,string Mode);
    }
}
