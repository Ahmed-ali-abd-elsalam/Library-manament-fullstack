namespace Application.Services
{
    public interface INotificationService
    {
        Task SendToUserAsync(string userId, string message);
        Task SendToAdminsAsync(string message);
    }

}
