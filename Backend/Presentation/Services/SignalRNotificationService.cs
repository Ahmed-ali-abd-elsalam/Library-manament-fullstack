using Application.Common;
using Application.Services;
using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;

namespace Presentation.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, string message)
        {
            await _hubContext.Clients.User(userId).ReceiveUserNotification(message);
        }

        public async Task SendToAdminsAsync(string message)
        {
            await _hubContext.Clients.All.ReceiveAdminNotification(message);
        }
    }
}
