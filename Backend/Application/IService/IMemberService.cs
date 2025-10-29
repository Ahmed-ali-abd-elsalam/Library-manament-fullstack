using Application.DTOs;
using Application.Results;
using Domain.Entities;

namespace Application.IService
{
    public interface IMemberService
    {
        public Task<Result<PaginatedMemberResponseDto>> GetMembers(int offset, int pagesize, MembersFilter membersFilter);
        public Task<Result<MemberResponseDto>> AddMember(RegisterMemberDto memberDto);
        public Task<Result> editMember(string Email, Member newMember);
        public Task<Result<MemberResponseDto>> GetMember(string Email);


    }
}
