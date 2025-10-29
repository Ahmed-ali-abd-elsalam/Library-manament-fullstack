using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IMemberRepository
    {
        public Task<bool> CheckExistsAsyncById(string Id);
        public Task<int> GetTotalCountAsync(MembersFilter membersfilter);

        public Task<bool> CheckExistsAsyncByEmail(string Email);
        public Task<ICollection<Member>> GetMembersAsync(MembersFilter membersfilter, int offset, int pagesize);
        public Task<Member?> GetMemberAsyncById(string Id);
        public Task<Member?> GetMemberAsyncByEmail(string Email);
        public Task<bool> editMemberAsync(string Email, Member newMember);
        public Task<Member> AddMemberAsync(Member member);
    }
}
