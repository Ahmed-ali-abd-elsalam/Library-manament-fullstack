using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _repository;

        public MemberService(IMemberRepository repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<MemberResponseDto>> GetMembers()
        {
            var members =  await _repository.GetMembersAsync();
            List<MemberResponseDto> membersResponse = [];
            foreach (var member in members)
            {
                membersResponse.Add(member.ToMemberResponseDto());
            }
            return membersResponse;
        }
        
        public async Task<MemberResponseDto> AddMember(RegisterMemberDto memberDto)
        {
            Member member = memberDto.RegisterDtoToMember();
            var hashedPassword = new PasswordHasher<Member>().HashPassword(member,memberDto.Password);
            member.HashedPassword = hashedPassword;
            await _repository.AddMemberAsync(member);
            return member.ToMemberResponseDto();
        }


    }
}
