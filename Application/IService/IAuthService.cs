using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IAuthService
    {
        Task<string> Login(LoginMemberDto loginMemberDto);
        Task<MemberResponseDto> Signup(RegisterMemberDto registerMemberDto);
        string createToken(Member member);
    }
}
