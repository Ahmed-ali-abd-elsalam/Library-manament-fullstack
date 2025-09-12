using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    internal static class MemberMapper
    {
        public static Member RegisterDtoToMember(this RegisterMemberDto memberDto)
        {
            return new Member {
                Email = memberDto.Email ,
                HashedPassword = memberDto.Password,
                Name =memberDto.Name,
                Role = memberDto.Role,
                JoinDate=DateOnly.FromDateTime(DateTime.UtcNow)};
        }
        public static MemberResponseDto ToMemberResponseDto(this Member member)
        {
            return new MemberResponseDto
            {
                Email = member.Email,
                Role = member.Role,
                Name = member.Name,
            };
        }
    }
}
