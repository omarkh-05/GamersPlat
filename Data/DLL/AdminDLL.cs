using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class AdminDLL
    {
        // ================ Admin Analytics ================
        public static async Task<DTO_AdminStatistics> GetSystemStatistics()
        {
            return await GetSystemStatistics(null, null);
        }
        // ================ Admin Analytics ================


        // ================ Advanced analytics with time filters ================
        public static async Task<DTO_AdminStatistics> GetSystemStatistics(DateTime? from,DateTime? to)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                // ============================================================
                // USERS
                // ============================================================
                var usersQuery = db.Users
                    .AsNoTracking();
                var totalUsers = await usersQuery.CountAsync();
                var activeUsers = await usersQuery
                    .CountAsync(u => u.IsActive);
                var last30Days = DateTime.UtcNow.AddDays(-30);
                var newRegistrations = await usersQuery
                    .CountAsync(u => u.CreatedAt >= last30Days);
                var blockedUsers = await usersQuery
                    .CountAsync(u => !u.IsActive);

                // ============================================================
                // CENTERS
                // ============================================================
                var centersQuery = db.Centers
                    .AsNoTracking();
                var totalCenters = await centersQuery.CountAsync();
                var activeCenters = await centersQuery
                    .CountAsync(c => c.IsActive == true);
                var pendingCenters = await centersQuery
                    .CountAsync(c => c.CenterStatus == "Pending");

                // ============================================================
                // TOP RATED CENTER
                // ============================================================
                var topRatedCenter = await db.Reviews
                    .AsNoTracking()
                    .GroupBy(r => new
                    {
                        r.CenterId,
                        r.Center.CenterName
                    })
                    .Select(g => new DTO_TopRatedCenter
                    {
                        CenterId = g.Key.CenterId,
                        CenterName = g.Key.CenterName,
                        Rating = (decimal)g.Average(r => r.Rating)
                    })
                    .OrderByDescending(x => x.Rating)
                    .FirstOrDefaultAsync();

                // ============================================================
                // BOOKINGS
                // ============================================================
                var bookingsQuery = db.Bookings
                    .AsNoTracking();
                if (from.HasValue)
                {
                    bookingsQuery = bookingsQuery
                        .Where(b => b.CreatedAt >= from.Value);
                }
                if (to.HasValue)
                {
                    bookingsQuery = bookingsQuery
                        .Where(b => b.CreatedAt <= to.Value);
                }
                var totalBookings = await bookingsQuery
                    .CountAsync();
                var totalRevenue = await bookingsQuery
                    .Where(b => b.Status != "Cancelled")
                    .SumAsync(b => (decimal?)b.TotalPrice) ?? 0m;

                // ============================================================
                // TOP PERFORMING CENTERS
                // ============================================================
                var topPerformingCenters = await bookingsQuery
                    .Where(b => b.Status != "Cancelled")
                    .GroupBy(b => new
                    {
                        b.CenterId,
                        b.Center.CenterName,
                        b.Center.IsActive,
                        CityName = b.Center.City.Name,
                        OwnerName = b.Center.OwnerUser != null
                            ? b.Center.OwnerUser.FullName
                            : ""
                    })
                    .Select(g => new DTO_TopPerformingCenter
                    {
                        CenterId = g.Key.CenterId,
                        CenterName = g.Key.CenterName,
                        CityName = g.Key.CityName,
                        OwnerName = g.Key.OwnerName,
                        Bookings = g.Count(),
                        Revenue = g.Sum(b => b.TotalPrice),
                        IsActive = g.Key.IsActive == true
                    })
                    .OrderByDescending(x => x.Bookings)
                    .Take(3)
                    .ToListAsync();

                // ============================================================
                // SESSIONS
                // ============================================================
                var sessionsQuery = db.Sessions
                    .AsNoTracking();
                if (from.HasValue)
                {
                    sessionsQuery = sessionsQuery
                        .Where(s => s.CreatedAt >= from.Value);
                }
                if (to.HasValue)
                {
                    sessionsQuery = sessionsQuery
                        .Where(s => s.CreatedAt <= to.Value);
                }
                var totalSessions = await sessionsQuery
                    .CountAsync(s => s.SessionStatus == "Completed");

                // ============================================================
                // POPULAR DEVICE
                // ============================================================
                var popularDevice = await sessionsQuery
                    .Where(s => s.SessionStatus == "Completed")
                    .GroupBy(s => new
                    {
                        s.DeviceId,
                        s.Device.DeviceName
                    })
                    .Select(g => new
                    {
                        DeviceName = g.Key.DeviceName,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefaultAsync();
                var popularDevicePercentage =
                    totalSessions > 0 && popularDevice != null
                        ? (int)Math.Round(
                            (decimal)popularDevice.Count /
                            totalSessions * 100)
                        : 0;

                // ============================================================
                // POPULAR LOCATION
                // ============================================================
                var validBookingsQuery = bookingsQuery
                    .Where(b => b.Status != "Cancelled");
                var validBookingCount = await validBookingsQuery
                    .CountAsync();
                var popularLocation = await validBookingsQuery
                    .GroupBy(b => b.Center.City.Name)
                    .Select(g => new
                    {
                        CityName = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefaultAsync();

                var popularLocationPercentage =
                    validBookingCount > 0 && popularLocation != null
                        ? (int)Math.Round(
                            (decimal)popularLocation.Count /
                            validBookingCount * 100)
                        : 0;

                // ============================================================
                // REVENUE GROWTH
                // ============================================================
                decimal revenueGrowth = 0;
                if (from.HasValue && to.HasValue)
                {
                    var periodLength = to.Value - from.Value;
                    var previousFrom = from.Value - periodLength;
                    var previousTo = from.Value;
                    var previousRevenue = await db.Bookings
                        .AsNoTracking()
                        .Where(b =>
                            b.CreatedAt >= previousFrom &&
                            b.CreatedAt < previousTo &&
                            b.Status != "Cancelled")
                        .SumAsync(b => (decimal?)b.TotalPrice) ?? 0m;
                    if (previousRevenue > 0)
                    {
                        revenueGrowth =
                            ((totalRevenue - previousRevenue) /
                             previousRevenue) * 100;
                    }
                }

                // ============================================================
                // RESULT
                // ============================================================
                return new DTO_AdminStatistics
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    NewRegistrations = newRegistrations,
                    BlockedUsers = blockedUsers,

                    TotalCenters = totalCenters,
                    ActiveCenters = activeCenters,
                    PendingCenters = pendingCenters,
                    TopRatedCenter = topRatedCenter,

                    TopPerformingCenters = topPerformingCenters,

                    TotalBookings = totalBookings,
                    TotalSessions = totalSessions,

                    PopularDevice = popularDevice?.DeviceName,
                    PopularDevicePercentage = popularDevicePercentage,

                    PopularLocation = popularLocation?.CityName,
                    PopularLocationPercentage = popularLocationPercentage,

                    TotalRevenue = totalRevenue,
                    RevenueGrowth = revenueGrowth
                };
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get System Statistics Error",ex);
                return new DTO_AdminStatistics();
            }
        }
        // ================ Advanced analytics with time filters ================


        // ================ Center approval/rejection ================
        public static async Task<bool> ApproveCenter(int centerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var center = await db.Centers.FirstOrDefaultAsync(c => c.CenterId == centerId);
                if (center == null) return false;

                center.CenterStatus = "Approved";
                center.IsActive = true;
                center.UpdatedAt = DateTime.UtcNow;

                var ok = await db.SaveChangesAsync() > 0;

                try
                {
                    if (center.OwnerUserId.HasValue)
                    {
                        var n = new Notification
                        {
                            UserId = center.OwnerUserId.Value,
                            CenterId = center.CenterId,
                            Title = "Center Approved",
                            Message = $"Your center '{center.CenterName}' has been approved by admin.",
                            Type = "Admin",
                            Status = "Unread",
                            ReferenceType = "Center",
                            CreatedAt = DateTime.UtcNow
                        };
                        await NotificationDLL.Add(n);
                    }
                }
                catch (Exception nEx)
                {
                    EventLog_Helper.WriteEventLog("Send Center Approved Notification Error", nEx);
                }

                return ok;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Approve Center Error", ex);
                return false;
            }
        }
        public static async Task<bool> RejectCenter(int centerId, string reason)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var center = await db.Centers.FirstOrDefaultAsync(c => c.CenterId == centerId);
                if (center == null) return false;

                center.CenterStatus = "Rejected";
                center.IsActive = false;
                center.UpdatedAt = DateTime.UtcNow;

                var ok = await db.SaveChangesAsync() > 0;

                try
                {
                    if (center.OwnerUserId.HasValue)
                    {
                        var n = new Notification
                        {
                            UserId = center.OwnerUserId.Value,
                            CenterId = center.CenterId,
                            Title = "Center Rejected",
                            Message = $"Your center '{center.CenterName}' was rejected. Reason: {reason}",
                            Type = "Admin",
                            Status = "Unread",
                            ReferenceType = "Center",
                            CreatedAt = DateTime.UtcNow
                        };
                        await NotificationDLL.Add(n);
                    }
                }
                catch (Exception nEx)
                {
                    EventLog_Helper.WriteEventLog("Send Center Rejected Notification Error", nEx);
                }

                EventLog_Helper.WriteEventLog($"Center {centerId} rejected by admin.", new Exception(reason));

                return ok;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Reject Center Error", ex);
                return false;
            }
        }
        // ================ Center approval/rejection ================


        // ================ Announcements ================
        public static async Task<bool> SendAnnouncement(string title, string message, string audience)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var usersQuery = db.Users.AsQueryable();
                if (audience == "Players") usersQuery = usersQuery.Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Player"));
                else if (audience == "Owners") usersQuery = usersQuery.Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Owner"));

                var users = await usersQuery.Select(u => new { u.UserId }).ToListAsync();
                foreach (var u in users)
                {
                    var n = new Notification
                    {
                        UserId = u.UserId,
                        Title = title,
                        Message = message,
                        Type = "Announcement",
                        Status = "Unread",
                        ReferenceType = "Announcement",
                        CreatedAt = DateTime.UtcNow
                    };
                    await NotificationDLL.Add(n);
                }
                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Send Announcement Error", ex);
                return false;
            }
        }
        // ================ Announcements ================


        // ================ Reports ================
        public static async Task<bool> SubmitReport(int reporterUserId, string reportType, int? referenceId, string reason, int? centerId = null)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var admins = await db.Users.Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Admin")).Select(u => new { u.UserId }).ToListAsync();
                foreach (var a in admins)
                {
                    var n = new Notification
                    {
                        UserId = a.UserId,
                        CenterId = centerId,
                        Title = "New Report",
                        Message = $"Reporter: {reporterUserId}. Type: {reportType}. RefId: {referenceId}. Reason: {reason}",
                        Type = "Report",
                        Status = "Open",
                        ReferenceType = "Report",
                        CreatedAt = DateTime.UtcNow
                    };
                    await NotificationDLL.Add(n);
                }
                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Submit Report Error", ex);
                return false;
            }
        }
        public static async Task<List<Notification>> GetReports()
        {
            try
            {
                var all = await NotificationDLL.GetAll();
                return all.Where(n => n.ReferenceType == "Report").ToList();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Reports Error", ex);
                return new List<Notification>();
            }
        }
        public static async Task<bool> ResolveReport(int notificationId, string action, int? relatedId = null)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var n = await NotificationDLL.GetByID(notificationId);
                if (n == null) return false;
                n.Status = "Resolved";
                var ok = await NotificationDLL.Update(n);
                if (!ok) return false;

                if (!string.IsNullOrWhiteSpace(action))
                {
                    if (action == "DeleteReview" && relatedId.HasValue) await ReviewDLL.Delete(relatedId.Value);
                    else if (action == "BlockUser" && relatedId.HasValue)
                    {
                        var user = await db.Users.FindAsync(relatedId.Value);
                        if (user != null)
                        {
                            user.IsActive = false;
                            await db.SaveChangesAsync();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Resolve Report Error", ex);
                return false;
            }
        }
        // ================ Reports ================
    }
}
