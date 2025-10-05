using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IConfirmEmailTokenRepository
    {
        Task<ConfirmEmailToken?> GetByEmailAsync(string email);
        Task<ConfirmEmailToken?> GetByIdAsync(Guid id);

        Task AddAsync(ConfirmEmailToken token);
        Task DeleteAsync(Guid id);
    }
}
