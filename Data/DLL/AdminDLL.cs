using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class AdminDLL
    {
        private readonly GamersPlatDbContext _db;
        private readonly ReviewDLL _reviewDLL;
        private readonly NotificationDLL _notificationDLL;

        public AdminDLL(GamersPlatDbContext db,ReviewDLL reviewDLL,NotificationDLL notificationDLL)
        {
            _db = db;
            _reviewDLL = reviewDLL;
            _notificationDLL = notificationDLL;
        }

        // ================ Admin Analytics ================
        public async Task<DTO_AdminStatistics> GetAdminDashboardStatistics( DateTime? from = null, DateTime? to = null)
        {
            return await GetAdminDashboard(from, to);
        }
        // ================ Admin Analytics ================


        // ================ Advanced analytics with time filters ================
        public async Task<DTO_AdminStatistics> GetAdminDashboard(DateTime? from, DateTime? to)
        {
            try
            {
                var last30Days = DateTime.UtcNow.AddDays(-30);

                return await _db.Users
                    .AsNoTracking()
                    .Select(u => new DTO_AdminStatistics
                    {
                        // Users
                        TotalUsers = _db.Users.Count(),
                        ActiveUsers = _db.Users.Count(x => x.IsActive),
                        NewRegistrations = _db.Users.Count(x => x.CreatedAt >= last30Days),
                        BlockedUsers = _db.Users.Count(x => !x.IsActive),

                        // Centers
                        TotalCenters = _db.Centers.Count(),
                        ActiveCenters = _db.Centers.Count(c => c.IsActive == true),
                        PendingCenters = _db.Centers.Count(c => c.CenterStatus == "Pending"),

                        // Bookings
                        TotalBookings = _db.Bookings.Count(b =>
                            (!from.HasValue || b.CreatedAt >= from.Value) &&
                            (!to.HasValue || b.CreatedAt <= to.Value)),

                        TotalRevenue = _db.Bookings
                            .Where(b =>
                                (!from.HasValue || b.CreatedAt >= from.Value) &&
                                (!to.HasValue || b.CreatedAt <= to.Value) &&
                                b.Status != "Cancelled")
                            .Sum(b => (decimal?)b.TotalPrice) ?? 0m,

                        // Top Performing Centers
                        TopPerformingCenters = _db.Bookings
                            .Where(b =>
                                (!from.HasValue || b.CreatedAt >= from.Value) &&
                                (!to.HasValue || b.CreatedAt <= to.Value) &&
                                b.Status != "Cancelled")
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
                            .ToList(),

                        // Sessions
                        TotalSessions = _db.Sessions.Count(s =>
                            s.SessionStatus == "Completed" &&
                            (!from.HasValue || s.CreatedAt >= from.Value) &&
                            (!to.HasValue || s.CreatedAt <= to.Value)),

                        // Popular Device
                        PopularDevice = _db.Sessions
                            .Where(s =>
                                s.SessionStatus == "Completed" &&
                                (!from.HasValue || s.CreatedAt >= from.Value) &&
                                (!to.HasValue || s.CreatedAt <= to.Value))
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
                            .Select(x => x.DeviceName)
                            .FirstOrDefault(),

                        PopularDevicePercentage = _db.Sessions
                            .Where(s =>
                                s.SessionStatus == "Completed" &&
                                (!from.HasValue || s.CreatedAt >= from.Value) &&
                                (!to.HasValue || s.CreatedAt <= to.Value))
                            .GroupBy(s => new
                            {
                                s.DeviceId,
                                s.Device.DeviceName
                            })
                            .Select(g => new
                            {
                                Count = g.Count()
                            })
                            .OrderByDescending(x => x.Count)
                            .Select(x => _db.Sessions.Count(s =>
                                    s.SessionStatus == "Completed" &&
                                    (!from.HasValue || s.CreatedAt >= from.Value) &&
                                    (!to.HasValue || s.CreatedAt <= to.Value)) > 0
                                ? (int)Math.Round(
                                    (decimal)x.Count /
                                    _db.Sessions.Count(s =>
                                        s.SessionStatus == "Completed" &&
                                        (!from.HasValue || s.CreatedAt >= from.Value) &&
                                        (!to.HasValue || s.CreatedAt <= to.Value)) * 100)
                                : 0)
                            .FirstOrDefault(),

                        // Popular Location
                        PopularLocation = _db.Bookings
                            .Where(b =>
                                b.Status != "Cancelled" &&
                                (!from.HasValue || b.CreatedAt >= from.Value) &&
                                (!to.HasValue || b.CreatedAt <= to.Value))
                            .GroupBy(b => b.Center.City.Name)
                            .Select(g => new
                            {
                                CityName = g.Key,
                                Count = g.Count()
                            })
                            .OrderByDescending(x => x.Count)
                            .Select(x => x.CityName)
                            .FirstOrDefault(),

                        PopularLocationPercentage = _db.Bookings
                            .Where(b =>
                                b.Status != "Cancelled" &&
                                (!from.HasValue || b.CreatedAt >= from.Value) &&
                                (!to.HasValue || b.CreatedAt <= to.Value))
                            .GroupBy(b => b.Center.City.Name)
                            .Select(g => new
                            {
                                Count = g.Count()
                            })
                            .OrderByDescending(x => x.Count)
                            .Select(x => _db.Bookings.Count(b =>
                                    b.Status != "Cancelled" &&
                                    (!from.HasValue || b.CreatedAt >= from.Value) &&
                                    (!to.HasValue || b.CreatedAt <= to.Value)) > 0
                                ? (int)Math.Round(
                                    (decimal)x.Count /
                                    _db.Bookings.Count(b =>
                                        b.Status != "Cancelled" &&
                                        (!from.HasValue || b.CreatedAt >= from.Value) &&
                                        (!to.HasValue || b.CreatedAt <= to.Value)) * 100)
                                : 0)
                            .FirstOrDefault(),

                        // Revenue Growth
                        RevenueGrowth = from.HasValue && to.HasValue
                    ? (
                        (_db.Bookings
                            .Where(b =>
                                b.CreatedAt >= from.Value &&
                                b.CreatedAt <= to.Value &&
                                b.Status != "Cancelled")
                            .Sum(b => (decimal?)b.TotalPrice) ?? 0m
                      ) -
                        (_db.Bookings
                            .Where(b =>
                                b.CreatedAt >= from.Value - (to.Value - from.Value) &&
                                b.CreatedAt < from.Value &&
                                b.Status != "Cancelled")
                            .Sum(b => (decimal?)b.TotalPrice) ?? 0m)
                      ) /
                      ((_db.Bookings
                          .Where(b =>
                              b.CreatedAt >= from.Value - (to.Value - from.Value) &&
                              b.CreatedAt < from.Value &&
                              b.Status != "Cancelled")
                          .Sum(b => (decimal?)b.TotalPrice) ?? 0m) == 0
                          ? 1m
                          : (_db.Bookings
                              .Where(b =>
                                  b.CreatedAt >= from.Value - (to.Value - from.Value) &&
                                  b.CreatedAt < from.Value &&
                                  b.Status != "Cancelled")
                              .Sum(b => (decimal?)b.TotalPrice) ?? 0m)
                      ) * 100m
                    : 0m
                    })
                    .FirstOrDefaultAsync() ?? new DTO_AdminStatistics();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Admin Dashboard Error", ex);
                return new DTO_AdminStatistics();
            }
        }
        // ================ Advanced analytics with time filters ================

        // ================ User Management ================
        public async Task<bool> BlockUser(int userId, int adminId = 0)
        {
            try
            {
                var existing = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (existing == null) return false;

                existing.IsActive = false;

                if (adminId > 0 && adminId != existing.UserId)
                {
                    existing.UpdatedBy = adminId;
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                EventLog_Helper.WriteEventLog("Block User Error",ex);
                return false;
            }
        }
        public async Task<bool> UnBlockUser(int userId, int adminId = 0)
        {
            try
            {
                var existing = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (existing == null) return false;

                existing.IsActive = true;

                if (adminId > 0 && adminId != existing.UserId)
                {
                    existing.UpdatedBy = adminId;
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("UnBlock User Error", ex);
                return false;
            }
        }
        // ================ User Management ================


        // ================ Center approval/rejection ================
        public async Task<bool> ApproveCenter(int centerId)
        {
            try
            {
                var center = await _db.Centers
                    .FirstOrDefaultAsync(c => c.CenterId == centerId);

                if (center == null)
                    return false;

                center.CenterStatus = "approved";
                center.IsActive = true;
                center.UpdatedAt = DateTime.Now;

                var ok = await _db.SaveChangesAsync() > 0;

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
                            CreatedAt = DateTime.Now
                        };

                        await _notificationDLL.Add(n);
                    }
                }
                catch (Exception nEx)
                {
                    EventLog_Helper.WriteEventLog("Send Center Approved Notification Error",nEx);
                }

                return ok;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Approve Center Error",
                    ex);

                return false;
            }
        }
        public async Task<bool> RejectCenter(int centerId, string reason)
        {
            try
            {
                var center = await _db.Centers
                    .FirstOrDefaultAsync(c => c.CenterId == centerId);

                if (center == null)
                    return false;

                center.CenterStatus = "rejected";
                center.IsActive = false;
                center.UpdatedAt = DateTime.Now;

                var ok = await _db.SaveChangesAsync() > 0;

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

                        await _notificationDLL.Add(n);
                    }
                }
                catch (Exception nEx)
                {
                    EventLog_Helper.WriteEventLog("Send Center Rejected Notification Error",nEx);
                }

                EventLog_Helper.WriteEventLog($"Center {centerId} rejected by admin.", new Exception(reason));

                return ok;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Reject Center Error",
                    ex);

                return false;
            }
        }
        // ================ Center approval/rejection ================


        // ================ Announcements ================
        public async Task<bool> SendAnnouncement( string title, string message, string audience)
        {
            try
            {
                var usersQuery = _db.Users.AsQueryable();

                if (audience == "Players")
                {
                    usersQuery = usersQuery
                        .Where(u => u.UserRoles.Any(
                            ur => ur.Role.RoleName == "Player"));
                }
                else if (audience == "Owners")
                {
                    usersQuery = usersQuery
                        .Where(u => u.UserRoles.Any(
                            ur => ur.Role.RoleName == "Owner"));
                }

                var users = await usersQuery
                    .Select(u => new { u.UserId })
                    .ToListAsync();

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

                    await _notificationDLL.Add(n);
                }

                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Send Announcement Error",
                    ex);

                return false;
            }
        }
        // ================ Announcements ================


        // ================ Reports ================
        public async Task<bool> SubmitReport( int reporterUserId, string reportType, int? referenceId, string reason, int? centerId = null)
        {
            try
            {
                var admins = await _db.Users
                    .Where(u => u.UserRoles.Any(
                        ur => ur.Role.RoleName == "Admin"))
                    .Select(u => new { u.UserId })
                    .ToListAsync();

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

                    await _notificationDLL.Add(n);
                }

                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Submit Report Error",
                    ex);

                return false;
            }
        }
        public async Task<List<Notification>> GetReports()
        {
            try
            {
                var all = await _notificationDLL.GetAll();

                return all
                    .Where(n => n.ReferenceType == "Report")
                    .ToList();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Reports Error",
                    ex);

                return new List<Notification>();
            }
        }
        public async Task<bool> ResolveReport( int notificationId, string action, int? relatedId = null)
        {
            try
            {
                var n = await _notificationDLL.GetByID(notificationId);

                if (n == null)
                    return false;

                n.Status = "Resolved";

                var ok = await _notificationDLL.Update(n);

                if (!ok)
                    return false;

                if (!string.IsNullOrWhiteSpace(action))
                {
                    if (action == "DeleteReview" && relatedId.HasValue)
                    {
                        await _reviewDLL.Delete(relatedId.Value);
                    }
                    else if (action == "BlockUser" && relatedId.HasValue)
                    {
                        var user = await _db.Users
                            .FindAsync(relatedId.Value);

                        if (user != null)
                        {
                            user.IsActive = false;
                            await _db.SaveChangesAsync();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Resolve Report Error",
                    ex);

                return false;
            }
        }
        // ================ Reports ================
    }
}