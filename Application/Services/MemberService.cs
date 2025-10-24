using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Application.Results;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _repository;
        private readonly UserManager<Member> _userManager;
        private readonly IUnitOfWork unitOfWork;

        public MemberService(IMemberRepository repository, UserManager<Member> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<Result<PaginatedMemberResponseDto>> GetMembers(int offset, int pagesize, MembersFilter membersFilter)
        {
            int total = await _repository.GetTotalCountAsync(membersFilter);
            bool HasNext = offset + 1 * pagesize < total;
            bool HasPrev = offset > 0;
            var members = await _repository.GetMembersAsync(membersFilter, offset, pagesize);
            List<MemberResponseDto> membersDtos = [];
            foreach (var member in members)
            {
                membersDtos.Add(member.ToMemberResponseDto());
            }
            return new PaginatedMemberResponseDto
            {
                members = membersDtos,
                Total = total,
                HasNext = HasNext,
                HasPrev = HasPrev,
                Offset = offset,
                pageSize = pagesize
            };
        }

        public async Task<Result<MemberResponseDto>> AddMember(RegisterMemberDto memberDto)
        {

            if (await _userManager.FindByEmailAsync(memberDto.Email) != null) return Errors.DoesntExist;
            Member member = memberDto.RegisterDtoToMember();
            var result = await _userManager.CreateAsync(member, memberDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(member, "Member");

                return member.ToMemberResponseDto();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    return new Error(error.Description);
                }
                Console.WriteLine(result.Errors);
                return Errors.PasswordNotSecure;
            }
        }

        public async Task<Result> editMember(string Email, Member newMember)
        {
            if (await _repository.editMemberAsync(Email, newMember))
                return Errors.DoesntExist;
            await unitOfWork.SaveChangesAsync();
            return Result.success();
        }


    }
}
