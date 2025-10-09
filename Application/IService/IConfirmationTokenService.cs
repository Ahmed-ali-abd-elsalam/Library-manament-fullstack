using Domain.Entities;

namespace Application.IService
{
    public interface IConfirmationTokenService
    {
        Task<ConfirmationToken> generateTokenAsync(string Email,string mode);
        Task<bool> ValidateTokenAsync(Guid Id,string Mode,string email);
    }
}