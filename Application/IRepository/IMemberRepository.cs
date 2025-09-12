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
        public Task<bool> CheckExistsAsync(int Id);
        public Task<bool> CheckExistsAsync(string Email);
        public Task<ICollection<Member>> GetMembersAsync();
        public Task<Member?> GetMemberAsync(int Id);
        public Task<Member?> GetMemberAsync(string Email);

        public Task<Member> AddMemberAsync(Member member);
    }
}
