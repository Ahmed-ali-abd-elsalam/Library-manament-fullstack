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
        private readonly UserManager<Member> _userManager;

        public MemberService(IMemberRepository repository, UserManager<Member> userManager)
        {
            _repository = repository;
            _userManager = userManager;
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

            if (await _userManager.FindByEmailAsync(memberDto.Email) != null) return null;
            Member member = memberDto.RegisterDtoToMember();
            var result = await _userManager.CreateAsync(member, memberDto.Password);
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

        public async Task<bool> editMember(string Email,Member newMember)
        {
            return await _repository.editMemberAsync(Email, newMember);
        }


    }
}
