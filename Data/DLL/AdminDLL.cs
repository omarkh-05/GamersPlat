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

        public AdminDLL(GamersPlatDbContext db)
        {
            _db = db;
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

                       /* // Sessions
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
                       */

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
                center.IsApproved = true;
                center.UpdatedAt = DateTime.Now;

                var ok = await _db.SaveChangesAsync() > 0;

                return ok;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Approve Center Error",ex);

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
                center.IsApproved = false;
                center.IsActive = false;
                center.UpdatedAt = DateTime.Now;

                var ok = await _db.SaveChangesAsync() > 0;

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


    }
}