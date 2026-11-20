using Domain.DTOs.Notification;

namespace Bussiness.Interfaces;

public interface INotificationRealtimePublisher
{
    Task PublishAsync(int recipientUserId, DTO_Notification notification);
}
