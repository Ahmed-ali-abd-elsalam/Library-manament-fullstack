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
    public class ConfirmationTokenRepository : IConfirmationTokenRepository
    {
        private readonly LibraryDbContext _context;

        public ConfirmationTokenRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<ConfirmationToken?> GetByEmailAsync(string email, string mode)
        {
            return await _context.ConfirmationToken.Where(rt => rt.Mode == mode).OrderByDescending(rt => rt.CreatedAt)
                .FirstOrDefaultAsync(t => t.Email == email);
        }

        public async Task AddAsync(ConfirmationToken token)
        {
            await _context.ConfirmationToken.AddAsync(token);
        }

        public async Task DeleteAsync(Guid id)
        {
            var token = await _context.ConfirmationToken
                .FirstOrDefaultAsync(t => t.id == id);
            if (token != null)
            {
                _context.ConfirmationToken.Remove(token);
            }
        }

        public async Task<ConfirmationToken?> GetByIdAsync(Guid id,string mode)
        {
            return await _context.ConfirmationToken.Where(rt=> rt.Mode ==mode).OrderByDescending(rt=>rt.CreatedAt)
                .FirstOrDefaultAsync(t => t.id == id);
        }
    }
}
