using Application.DTOs;
using Application.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IMemberService
    {
        public  Task<Result<PaginatedMemberResponseDto>> GetMembers(int offset, int pagesize, MembersFilter membersFilter);
        public  Task<Result<MemberResponseDto>> AddMember(RegisterMemberDto memberDto);
        public Task<Result> editMember(string Email, Member newMember);

    }
}
