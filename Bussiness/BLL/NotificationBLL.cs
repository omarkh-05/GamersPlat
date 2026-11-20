using Bussiness.Enums;
using Bussiness.Interfaces;
using Data;
using Data.DLL;
using DataLayer;
using Domain.DTOs.Notification;

namespace Bussiness;

public class NotificationBLL : INotificationService
{
    private const string BookingType = "Booking";
    private const string CenterType = "Center";
    private readonly NotificationDLL _notificationDLL;
    private readonly INotificationRealtimePublisher _realtimePublisher;

    public NotificationBLL(NotificationDLL notificationDLL, INotificationRealtimePublisher realtimePublisher)
    {
        _notificationDLL = notificationDLL;
        _realtimePublisher = realtimePublisher;
    }

    public async Task NotifyBookingManagersAsync(int bookingId, string action)
    {
        try
        {
            var booking = await _notificationDLL.GetBookingForNotification(bookingId);
            if (booking == null)
                return;

            var recipients = await _notificationDLL.GetCenterBookingManagerUserIds(
                booking.CenterId, (short)Permissions.AcceptRejectBooking);
            recipients.Add(booking.Center.OwnerUserId);

            var eventName = action switch
            {
                "Created" => "created",
                "Cancelled" => "cancelled",
                "Updated" => "updated",
                _ => throw new ArgumentOutOfRangeException(nameof(action))
            };
            var title = $"Booking {eventName}";
            var message = $"Player {booking.User.FullName} {eventName} booking #{booking.BookingId} at {booking.Center.CenterName} for {booking.BookingDate:yyyy-MM-dd}, {booking.StartTime:HH:mm}-{booking.EndTime:HH:mm}. Total: {booking.TotalPrice:C}.";

            await CreateAsync(recipients, booking.CenterId, title, message, BookingType,
                $"Booking{action}:{booking.BookingId}");
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Booking Managers Error", ex);
        }
    }

    public async Task NotifyBookingDecisionAsync(int bookingId, int actorUserId, bool accepted)
    {
        try
        {
            var booking = await _notificationDLL.GetBookingForNotification(bookingId);
            if (booking == null)
                return;

            var action = accepted ? "accepted" : "rejected";
            var actor = accepted ? booking.AcceptedByNavigation?.FullName : booking.RejectedByNavigation?.FullName;
            var actorText = string.IsNullOrWhiteSpace(actor) ? "the center team" : actor;
            await CreateAsync(new[] { booking.UserId }, booking.CenterId,
                $"Booking {action}",
                $"Your booking #{booking.BookingId} at {booking.Center.CenterName} for {booking.BookingDate:yyyy-MM-dd}, {booking.StartTime:HH:mm}-{booking.EndTime:HH:mm} was {action} by {actorText}.",
                BookingType,
                $"Booking{(accepted ? "Accepted" : "Rejected")}:{booking.BookingId}");
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Booking Decision Error", ex);
        }
    }

    public async Task NotifyBookingCompletedAsync(int bookingId, int invoiceNumber)
    {
        try
        {
            var booking = await _notificationDLL.GetBookingForNotification(bookingId);
            if (booking == null)
                return;

            await CreateAsync(new[] { booking.Center.OwnerUserId }, booking.CenterId,
                "Booking completed",
                $"Booking #{booking.BookingId} for player {booking.User.FullName} at {booking.Center.CenterName} was stopped and invoice #{invoiceNumber} was generated.",
                BookingType,
                $"BookingCompleted:{booking.BookingId}");
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Booking Completion Error", ex);
        }
    }

    public async Task NotifyCenterCreatedAsync(int centerId)
    {
        try
        {
            var center = await _notificationDLL.GetCenterForNotification(centerId);
            if (center == null)
                return;

            var admins = await _notificationDLL.GetAdminUserIds();
            await CreateAsync(admins, center.CenterId,
                "New center submitted",
                $"Owner {center.OwnerUserId} created center '{center.CenterName}' for review.",
                CenterType,
                $"CenterCreated:{center.CenterId}");
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Admins Of New Center Error", ex);
        }
    }

    public async Task NotifyCenterStatusAsync(int centerId, string status)
    {
        try
        {
            var center = await _notificationDLL.GetCenterForNotification(centerId);
            if (center == null)
                return;

            var (title, message, reference) = status.ToLowerInvariant() switch
            {
                "accepted" => ("Center accepted", $"Your center '{center.CenterName}' has been accepted by admin.", "CenterAccepted"),
                "rejected" => ("Center rejected", $"Your center '{center.CenterName}' was rejected by admin.", "CenterRejected"),
                "activated" => ("Center activated", $"Your center '{center.CenterName}' has been activated.", "CenterActivated"),
                "deactivated" => ("Center deactivated", $"Your center '{center.CenterName}' has been deactivated.", "CenterDeactivated"),
                _ => throw new ArgumentOutOfRangeException(nameof(status))
            };

            await CreateAsync(new[] { center.OwnerUserId }, center.CenterId, title, message,
                CenterType, $"{reference}:{center.CenterId}");
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Center Owner Error", ex);
        }
    }

    public async Task<IReadOnlyList<DTO_Notification>> GetForUserAsync(int userId)
        => await _notificationDLL.GetForUser(userId);

    public Task<int> GetUnreadCountAsync(int userId)
        => _notificationDLL.GetUnreadCount(userId);

    public Task<bool> MarkAsReadAsync(int userId, int notificationId)
        => _notificationDLL.MarkAsRead(userId, notificationId);

    public Task<int> MarkAllAsReadAsync(int userId)
        => _notificationDLL.MarkAllAsRead(userId);

    private async Task CreateAsync(IEnumerable<int> recipientIds, int? centerId, string title,
        string message, string type, string referenceType)
    {
        var now = DateTime.UtcNow;
        var notifications = recipientIds
            .Where(userId => userId > 0)
            .Distinct()
            .Select(userId => new Notification
            {
                UserId = userId,
                CenterId = centerId,
                Title = title,
                Message = message,
                Type = type,
                Status = "Unread",
                ReferenceType = referenceType,
                ReadAt = null,
                CreatedAt = now
            })
            .ToList();

        var savedNotifications = await _notificationDLL.AddManyIfNew(notifications);
        foreach (var notification in savedNotifications)
        {
            var recipientUserId = notifications
                .First(item => item.NotificationId == notification.NotificationId)
                .UserId;

            try
            {
                await _realtimePublisher.PublishAsync(recipientUserId, notification);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Send Realtime Notification Error", ex);
            }
        }
    }
}
