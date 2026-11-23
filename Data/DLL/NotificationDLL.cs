using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Center;
using Domain.DTOs.Notification;
using Domain.DTOs.Pagination;
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

        // ================ CRUD ================
        public async Task<List<DTO_Notification>> GetTopUnReadForUserBell(int userId)
        {
            try
            {
                return await _db.Notifications
                    .AsNoTracking()
                    .Where(n => n.UserId == userId && n.Status == "unread")
                    .OrderByDescending(n => n.CreatedAt)
                    .ThenByDescending(n => n.NotificationId)
                    .Take(10)
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
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While GetTopUnReadForUserBell :", ex);
                throw;
            }
        }
        public async Task<List<DTO_Notification>> GetForUser(int userId, PaginationParameters pagination)
        {
            try
            {
                var normalized = pagination.Normalize();
                var skip = (int)Math.Min((long)(normalized.PageNumber - 1) * normalized.PageSize, int.MaxValue);

                return await _db.Notifications
                    .AsNoTracking()
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ThenByDescending(n => n.NotificationId)
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
                    .Skip(skip)
                    .Take(normalized.PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While GetForUser :", ex);
                throw;
            }
        }
        public async Task<List<DTO_Notification>> AddManyIfNew(List<Notification> notifications)
        {
            try
            {
                if (notifications.Count == 0)
                    return new List<DTO_Notification>();

                var tx = _db.Database.CurrentTransaction;
                var ownsTransaction = tx == null;

                if (ownsTransaction)
                    tx = await _db.Database.BeginTransactionAsync(
                        System.Data.IsolationLevel.Serializable);

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
                        .Where(n => !existingKeys.Contains(
                            (n.UserId, n.Type, n.ReferenceType)))
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
                EventLog_Helper.WriteEventLog("Error While AddManyIfNew :", ex);
                throw;
            }
        }
        public async Task<int> GetUnreadCount(int userId)
        {
            try
            {
                return await _db.Notifications.CountAsync(n => n.UserId == userId && n.Status == "unread");
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While GetUnreadCount :", ex);
                throw;
            }
        }
        // ================ CRUD ================


        // ================ Actions ================
        public async Task<bool> MarkAsRead(int userId, int notificationId)
        {
            try
            {
                var notification = await _db.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);
                if (notification == null)
                    return false;

                if (notification.Status != "unread")
                    return true;

                notification.Status = "read";
                notification.ReadAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While MarkAsRead :", ex);
                throw;
            }
        }
        public async Task<int> MarkAllAsRead(int userId)
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _db.Notifications
                    .Where(n => n.UserId == userId && n.Status == "unread")
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(n => n.Status, "read")
                        .SetProperty(n => n.ReadAt, now));
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While MarkAllAsRead :", ex);
                throw;
            }
        }
        // ================ Actions ================


        // ================ Notifications ================
        public async Task<DTO_BookingNotification?> GetBookingForNotification(int bookingId)
        {
            try
            {
                return await _db.Bookings
                    .AsNoTracking()
                    .Where(b => b.BookingId == bookingId)
                    .Select(b => new DTO_BookingNotification
                    {
                        BookingId = b.BookingId,
                        CenterId = b.CenterId,
                        UserId = b.UserId,
                        OwnerUserId = b.Center.OwnerUserId,

                        PlayerName = b.User.FullName,
                        CenterName = b.Center.CenterName,

                        BookingDate = b.BookingDate,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        TotalPrice = b.TotalPrice,

                        AcceptedByName = b.AcceptedByNavigation != null
                            ? b.AcceptedByNavigation.FullName : null,

                        RejectedByName = b.RejectedByNavigation != null
                            ? b.RejectedByNavigation.FullName : null
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While GetBookingForNotification :", ex);
                throw;
            }
        }
        public async Task<DTO_CenterDetails?> GetCenterForNotification(int centerId)
        {
            try
            {
                return await _db.Centers
                    .AsNoTracking()
                    .Select(c => new DTO_CenterDetails
                    {
                        CenterId = c.CenterId,
                        Name = c.CenterName,
                        OwnerId = c.OwnerUserId
                    })
                    .FirstOrDefaultAsync(c => c.CenterId == centerId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While GetCenterForNotification :", ex);
                throw;
            }
        }
        public async Task<List<int>> GetCenterBookingManagerUserIds(int centerId, short permission)
        {
            try
            {
                return await _db.Staff
                    .Where(s => s.CenterId == centerId && s.IsActive && s.Status.ToLower() == "active"
                        && (s.Permissions & permission) == permission && s.User.IsActive)
                    .Select(s => s.UserId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While GetCenterBookingManagerUserIds :", ex);
                throw;
            }
        }
        public async Task<List<int>> GetAdminUserIds()
        {
            try
            {
                return await _db.UserRoles
                    .Where(ur => ur.Role.RoleName.ToLower() == "admin")
                    .Select(ur => ur.UserId)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While GetAdminUserIds :", ex);
                throw;
            }
        }
        // ================ Notifications ================
    }
}