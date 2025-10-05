using Application.IRepository;
using Domain.Entities;
using FluentEmail.Core;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ConfirmEmailTokenRepository : IConfirmEmailTokenRepository
    {
        private readonly LibraryDbContext _context;

        public ConfirmEmailTokenRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<ConfirmEmailToken?> GetByEmailAsync(string email)
        {
            return await _context.confirmEmailTokens
                .FirstOrDefaultAsync(t => t.Email == email);
        }

        public async Task AddAsync(ConfirmEmailToken token)
        {
            await _context.confirmEmailTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var token = await _context.confirmEmailTokens
                .FirstOrDefaultAsync(t => t.id == id);
            if (token != null)
            {
                _context.confirmEmailTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ConfirmEmailToken?> GetByIdAsync(Guid id)
        {
            return await _context.confirmEmailTokens
                .FirstOrDefaultAsync(t => t.id == id);
        }
    }
}
