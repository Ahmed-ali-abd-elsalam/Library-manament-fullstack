namespace Application.Common
{
    public interface INotificationClient
    {
        Task ReceiveUserNotification(string message);
        Task ReceiveAdminNotification(string message);
    }
}