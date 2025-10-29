using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IConfirmationTokenRepository
    {
        Task<ConfirmationToken?> GetByEmailAsync(string email, string mode);
        Task<ConfirmationToken?> GetByIdAsync(Guid id,string mode);

        Task AddAsync(ConfirmationToken token);
        Task DeleteAsync(Guid id);
    }
}
