using Bussiness.Enums;
using Bussiness.Interfaces;
using Data;
using Data.DLL;
using DataLayer;
using Domain.DTOs.Notification;
using Domain.DTOs.Pagination;

namespace Bussiness;

public class NotificationBLL : INotificationService
{
    private const string BookingType = "booking";
    private const string CenterType = "center";
    private readonly NotificationDLL _notificationDLL;
    private readonly INotificationRealtimePublisher _realtimePublisher;

    public NotificationBLL(NotificationDLL notificationDLL, INotificationRealtimePublisher realtimePublisher)
    {
        _notificationDLL = notificationDLL;
        _realtimePublisher = realtimePublisher;
    }

    // ================ CRUD ================
    public async Task<IReadOnlyList<DTO_Notification>> GetTopUnReadForUserBell(int userId)
        => await _notificationDLL.GetTopUnReadForUserBell(userId);
    public async Task<IReadOnlyList<DTO_Notification>> GetForUserAsync(int userId, PaginationParameters pagination)
        => await _notificationDLL.GetForUser(userId, pagination);
    public async Task<int> GetUnreadCountAsync(int userId)
        => await _notificationDLL.GetUnreadCount(userId);
    private async Task CreateAsync(IEnumerable<int> recipientIds, int? centerId, string title, string message, string type, string referenceType)
    {
        var now = DateTime.Now;
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
                Status = "unread",
                ReferenceType = referenceType,
                ReadAt = null,
                CreatedAt = now
            })
            .ToList();

        if (notifications.Count == 0)
            return;

        var savedNotifications = await _notificationDLL.AddManyIfNew(notifications);

        var userIdsByNotificationId = notifications.ToDictionary(n => n.NotificationId, n => n.UserId);

        foreach (var notification in savedNotifications)
        {
            if (!userIdsByNotificationId.TryGetValue(notification.NotificationId, out var recipientUserId))
                continue;
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
    // ================ CRUD ================


    // ================ Actions ================
    public Task<bool> MarkAsReadAsync(int userId, int notificationId)
        => _notificationDLL.MarkAsRead(userId, notificationId);
    public Task<int> MarkAllAsReadAsync(int userId)
        => _notificationDLL.MarkAllAsRead(userId);
    // ================ Actions ================


    // ================ Notifications ================
    public async Task NotifyBookingManagersAsync(int bookingId, string action)
    {
        try
        {
            var booking = await _notificationDLL.GetBookingForNotification(bookingId);
            if (booking == null)
                return;

            var recipients = await _notificationDLL.GetCenterBookingManagerUserIds(booking.CenterId, (short)Permissions.AcceptRejectBooking);
            recipients.Add(booking.OwnerUserId);

            var title = $"Booking {action.ToLower()}";
            var message = $"Player {booking.PlayerName} {action.ToLower()} booking #{booking.BookingId} at {booking.CenterName} for {booking.BookingDate:yyyy-MM-dd}, {booking.StartTime:HH:mm}-{booking.EndTime:HH:mm}. Total: {booking.TotalPrice:C}.";

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
            var actor = accepted ? booking.AcceptedByName : booking.RejectedByName;
            var actorText = string.IsNullOrWhiteSpace(actor) ? "the center team" : actor;
            await CreateAsync(new[] { booking.UserId }, booking.CenterId,
                $"Booking {action}",
                $"Your booking #{booking.BookingId} at {booking.CenterName} for {booking.BookingDate:yyyy-MM-dd}, {booking.StartTime:HH:mm}-{booking.EndTime:HH:mm} was {action} by {actorText}.",
                BookingType,
                $"Booking{(accepted ? "accepted" : "rejected")}:{booking.BookingId}");
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Booking Decision Error", ex);
        }
    }
    public async Task NotifyBookingTimeExpiredAsync(int bookingId)
    {
        var booking = await _notificationDLL.GetBookingForNotification(bookingId);

        if (booking == null)
            return;

        var recipients = await _notificationDLL.GetCenterBookingManagerUserIds(booking.CenterId, (short)Permissions.AcceptRejectBooking);

        // إضافة Owner
        recipients.Add(booking.OwnerUserId);

        // إزالة التكرار
        recipients = recipients.Distinct().ToList();

        await CreateAsync(
            recipients,
            booking.CenterId,
            "Booking Time Expired",
            $"The reserved time for booking #{booking.BookingId} has expired. The booking is still running.",
            BookingType,
            $"BookingTimeExpired:{bookingId}"
        );
    }
    public async Task NotifyBookingCompletedAsync(int bookingId, int invoiceNumber)
    {
        try
        {
            var booking = await _notificationDLL.GetBookingForNotification(bookingId);
            if (booking == null)
                return;

            await CreateAsync(
                new[] { booking.OwnerUserId }, booking.CenterId,
                "Booking completed",
                $"Booking #{booking.BookingId} for player {booking.PlayerName} at {booking.CenterName} was stopped and invoice #{invoiceNumber} was generated.",
                BookingType,
                $"BookingCompleted:{booking.BookingId}");
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Booking Completion Error", ex);
        }
    }
    public async Task NotifyBookingAutoCancelledAsync(int bookingId)
    {
        try
        {
            var booking = await _notificationDLL.GetBookingForNotification(bookingId);

            if (booking == null)
                return;

            await CreateAsync(
                new[] { booking.UserId },
                booking.CenterId,
                "Booking Automatically Cancelled",
                $"Your booking #{booking.BookingId} at {booking.CenterName} was automatically cancelled because it was not started within 15 minutes of its scheduled start time.",
                BookingType,
                $"BookingAutoCancelled:{booking.BookingId}"
            );
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Booking Auto Cancellation Error", ex);
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
                $"Owner {center.OwnerId} created center '{center.Name}' for review.",
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
                "accepted" => ("Center accepted", $"Your center '{center.Name}' has been accepted by admin.", "center accepted"),
                "rejected" => ("Center rejected", $"Your center '{center.Name}' was rejected by admin.", "center rejected"),
                "activated" => ("Center activated", $"Your center '{center.Name}' has been activated.", "center activated"),
                "deactivated" => ("Center deactivated", $"Your center '{center.Name}' has been deactivated.", "center deactivated"),
                _ => throw new ArgumentOutOfRangeException(nameof(status))
            };

            await CreateAsync(new[] { center.OwnerId }, center.CenterId, title, message,
                CenterType, $"{reference}:{center.CenterId}");
        }
        catch (Exception ex)
        {
            EventLog_Helper.WriteEventLog("Notify Center Owner Error", ex);
        }
    }
    // ================ Notifications ================
}
