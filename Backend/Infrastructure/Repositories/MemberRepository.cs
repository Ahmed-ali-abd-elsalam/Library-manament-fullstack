using Application.DTOs;
using Application.IRepository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly LibraryDbContext _context;

        public MemberRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Member> AddMemberAsync(Member member)
        {
            await _context.Members.AddAsync(member);
            return member;
        }

        public async Task<bool> CheckExistsAsyncById(string Id)
        {
            return await _context.Members.AnyAsync(m => m.Id == Id);

        }
        public async Task<bool> CheckExistsAsyncByEmail(string Email)
        {
            return await _context.Members.AnyAsync(m => m.Email == Email);
        }
        public async Task<Member?> GetMemberAsyncById(string Id)
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.Id == Id);
        }
        public async Task<Member?> GetMemberAsyncByEmail(string Email)
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.Email == Email);
        }
        public async Task<ICollection<Member>> GetMembersAsync(MembersFilter memberFilter, int offset, int pagesize)
        {
            var query = _context.Members.AsQueryable();
            if (memberFilter.Email != string.Empty)
                query = query.Where(book => book.Email == memberFilter.Email);
            if (memberFilter.UserName != string.Empty)
                query = query.Where(book => book.UserName == memberFilter.UserName);
            if (memberFilter.PhoneNumber != string.Empty)
                query = query.Where(book => book.PhoneNumber == memberFilter.PhoneNumber);
            return await query.OrderBy(m => m.Id).Skip(offset * pagesize).Take(pagesize).ToListAsync();
        }

        public async Task<bool> editMemberAsync(string Email, Member newMember)
        {
            var member = await _context.Members
           .FirstOrDefaultAsync(m => m.Email == Email);

            if (member == null)
            {
                return false;
            }

            member.UserName = newMember.UserName;
            member.PhoneNumber = newMember.PhoneNumber;
            member.EmailConfirmed = newMember.EmailConfirmed;
            _context.Members.Update(member);
            return true;
        }

        public async Task<int> GetTotalCountAsync(MembersFilter memberFilter)
        {
            var query = _context.Members.AsQueryable();
            if (memberFilter.Email != string.Empty)
                query = query.Where(book => book.Email == memberFilter.Email);
            if (memberFilter.UserName != string.Empty)
                query = query.Where(book => book.UserName == memberFilter.UserName);
            if (memberFilter.PhoneNumber != string.Empty)
                query = query.Where(book => book.PhoneNumber == memberFilter.PhoneNumber);
            return await query.CountAsync();
        }
    }
}
