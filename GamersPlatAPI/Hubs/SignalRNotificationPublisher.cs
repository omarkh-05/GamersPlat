using Bussiness.Interfaces;
using Domain.DTOs.Notification;
using Microsoft.AspNetCore.SignalR;

namespace GamersPlatAPI.Hubs;

public sealed class SignalRNotificationPublisher : INotificationRealtimePublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task PublishAsync(int recipientUserId, DTO_Notification notification)
        => _hubContext.Clients
            .Group(NotificationHub.GetUserGroupName(recipientUserId))
            .SendAsync("ReceiveNotification", notification);
}
