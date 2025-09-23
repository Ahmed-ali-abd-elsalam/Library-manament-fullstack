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
                UserName =memberDto.Name,
                JoinDate=DateOnly.FromDateTime(DateTime.UtcNow)};
        }
        public static MemberResponseDto ToMemberResponseDto(this Member member)
        {
            return new MemberResponseDto
            {
                Id = member.Id,
                Email = member.Email,
                Name = member.UserName,
            };
        }
    }
}
