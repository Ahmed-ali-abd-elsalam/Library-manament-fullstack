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
    public class ConfirmEmailTokenService : IConfirmEmailTokenService
    {
        private readonly IConfirmEmailTokenRepository emailTokenRepository;

        public ConfirmEmailTokenService(IConfirmEmailTokenRepository emailTokenRepository)
        {
            this.emailTokenRepository = emailTokenRepository;
        }

        public async Task<ConfirmEmailToken> generateEmailTokenAsync(string Email)
        {
            ConfirmEmailToken confimationToken = new ConfirmEmailToken
            {
                CreatedAt = DateTime.UtcNow,
                Email = Email,
                expiresAt = DateTime.UtcNow.AddDays(1),
                id = Guid.NewGuid()
            };
            emailTokenRepository.AddAsync(confimationToken);
            return confimationToken;
        }
        public async Task<bool> ValidateEmailTokenAsync(Guid Id)
        {
            ConfirmEmailToken confimationToken = await emailTokenRepository.GetByIdAsync(Id);
            if (confimationToken == null || confimationToken.expiresAt < DateTime.UtcNow)
            {
                return false;
            }
            await emailTokenRepository.DeleteAsync(Id);
            return true;
        }
    }
}
