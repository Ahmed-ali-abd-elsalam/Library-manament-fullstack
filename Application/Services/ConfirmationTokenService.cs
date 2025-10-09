using Application.IRepository;
using Application.IService;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ConfirmationTokenService : IConfirmationTokenService
    {
        private readonly IConfirmationTokenRepository confirmationTokenRepository;

        public ConfirmationTokenService(IConfirmationTokenRepository ConfirmationTokenRepository)
        {
            confirmationTokenRepository = ConfirmationTokenRepository;
        }

        public async Task<ConfirmationToken> generateTokenAsync(string Email,string mode)
        {
            ConfirmationToken confimationToken = new ConfirmationToken
            {
                CreatedAt = DateTime.UtcNow,
                Email = Email,
                expiresAt = DateTime.UtcNow.AddDays(1),
                id = Guid.NewGuid(),
                Mode = mode
            };
            confirmationTokenRepository.AddAsync(confimationToken);
            return confimationToken;
        }
        public async Task<bool> ValidateTokenAsync(Guid Id,string Mode,string email)
        {
            ConfirmationToken confimationToken = await confirmationTokenRepository.GetByIdAsync(Id,Mode);
            if (confimationToken == null || confimationToken.expiresAt < DateTime.UtcNow || confimationToken.Email != email)
            {
                return false;
            }
            await confirmationTokenRepository.DeleteAsync(Id);
            return true;
        }
    }
}
