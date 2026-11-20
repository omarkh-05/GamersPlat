using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Notification;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class NotificationDLL
    {
        private readonly GamersPlatDbContext _db;

        public NotificationDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        public async Task<Booking?> GetBookingForNotification(int bookingId)
        {
            return await _db.Bookings
                .AsNoTracking()
                .Include(b => b.Center)
                .Include(b => b.User)
                .Include(b => b.AcceptedByNavigation)
                .Include(b => b.RejectedByNavigation)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<Center?> GetCenterForNotification(int centerId)
        {
            return await _db.Centers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CenterId == centerId);
        }

        public async Task<List<int>> GetCenterBookingManagerUserIds(int centerId, short permission)
        {
            return await _db.Staff
                .Where(s => s.CenterId == centerId && s.IsActive && s.Status.ToLower() == "active"
                    && (s.Permissions & permission) == permission && s.User.IsActive)
                .Select(s => s.UserId)
                .ToListAsync();
        }

        public async Task<List<int>> GetAdminUserIds()
        {
            return await _db.UserRoles
                .Where(ur => ur.Role.RoleName.ToLower() == "admin")
                .Select(ur => ur.UserId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<DTO_Notification>> GetForUser(int userId)
        {
            return await _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new DTO_Notification
                {
                    NotificationId = n.NotificationId,
                    CenterId = n.CenterId,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    Status = n.Status,
                    ReferenceType = n.ReferenceType,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public Task<int> GetUnreadCount(int userId)
            => _db.Notifications.CountAsync(n => n.UserId == userId && n.Status == "Unread");

        public async Task<bool> MarkAsRead(int userId, int notificationId)
        {
            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);
            if (notification == null)
                return false;

            if (notification.Status != "Unread")
                return true;

            notification.Status = "Read";
            notification.ReadAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> MarkAllAsRead(int userId)
        {
            var unread = await _db.Notifications
                .Where(n => n.UserId == userId && n.Status == "Unread")
                .ToListAsync();
            var now = DateTime.UtcNow;
            foreach (var notification in unread)
            {
                notification.Status = "Read";
                notification.ReadAt = now;
            }

            await _db.SaveChangesAsync();
            return unread.Count;
        }

        public async Task<List<DTO_Notification>> AddManyIfNew(List<Notification> notifications)
        {
            if (notifications.Count == 0)
                return new List<DTO_Notification>();

            try
            {
                var tx = _db.Database.CurrentTransaction;
                var ownsTransaction = tx == null;
                if (ownsTransaction)
                    tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

                try
                {
                    var recipientIds = notifications.Select(n => n.UserId).Distinct().ToList();
                    var eventTypes = notifications.Select(n => n.Type).Distinct().ToList();
                    var references = notifications.Select(n => n.ReferenceType).Distinct().ToList();
                    var existing = await _db.Notifications
                        .Where(n => recipientIds.Contains(n.UserId)
                            && eventTypes.Contains(n.Type)
                            && references.Contains(n.ReferenceType))
                        .Select(n => new { n.UserId, n.Type, n.ReferenceType })
                        .ToListAsync();

                    var existingKeys = existing
                        .Select(n => (n.UserId, n.Type, n.ReferenceType))
                        .ToHashSet();
                    var unique = notifications
                        .GroupBy(n => (n.UserId, n.Type, n.ReferenceType))
                        .Select(group => group.First())
                        .Where(n => !existingKeys.Contains((n.UserId, n.Type, n.ReferenceType)))
                        .ToList();

                    if (unique.Count > 0)
                    {
                        _db.Notifications.AddRange(unique);
                        await _db.SaveChangesAsync();
                    }

                    if (ownsTransaction)
                        await tx!.CommitAsync();

                    return unique.Select(n => new DTO_Notification
                    {
                        NotificationId = n.NotificationId,
                        CenterId = n.CenterId,
                        Title = n.Title,
                        Message = n.Message,
                        Type = n.Type,
                        Status = n.Status,
                        ReferenceType = n.ReferenceType,
                        ReadAt = n.ReadAt,
                        CreatedAt = n.CreatedAt
                    }).ToList();
                }
                catch
                {
                    if (ownsTransaction && tx != null)
                        await tx.RollbackAsync();
                    throw;
                }
                finally
                {
                    if (ownsTransaction && tx != null)
                        await tx.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Notifications Error", ex);
                return new List<DTO_Notification>();
            }
        }

    }
}