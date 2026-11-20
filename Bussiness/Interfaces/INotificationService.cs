using Domain.DTOs.Notification;

namespace Bussiness.Interfaces
{
    public interface INotificationService
    {
        Task NotifyBookingManagersAsync(int bookingId, string action);
        Task NotifyBookingDecisionAsync(int bookingId, int actorUserId, bool accepted);
        Task NotifyBookingCompletedAsync(int bookingId, int invoiceNumber);
        Task NotifyCenterCreatedAsync(int centerId);
        Task NotifyCenterStatusAsync(int centerId, string status);
        Task<IReadOnlyList<DTO_Notification>> GetForUserAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int userId, int notificationId);
        Task<int> MarkAllAsReadAsync(int userId);
    }
}
