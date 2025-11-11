using Microsoft.AspNetCore.SignalR;

namespace Presentation.MiddleWares
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // Use "sub" or "id" depending on your JWT payload
            return connection.User?.FindFirst("Id")?.Value;
        }
    }
}