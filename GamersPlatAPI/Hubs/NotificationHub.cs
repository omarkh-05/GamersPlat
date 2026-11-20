using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GamersPlatAPI.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public static string GetUserGroupName(int userId) => $"user:{userId}";

        public override async Task OnConnectedAsync()
        {
            var userIdValue = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdValue, out var userId) || userId <= 0)
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
            await base.OnConnectedAsync();
        }
    }
}
