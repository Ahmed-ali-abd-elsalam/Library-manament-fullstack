using Domain.Entities;

namespace Application.IService
{
    public interface IConfirmEmailTokenService
    {
        Task<ConfirmEmailToken> generateEmailTokenAsync(string Email);
        Task<bool> ValidateEmailTokenAsync(Guid Id);
    }
}