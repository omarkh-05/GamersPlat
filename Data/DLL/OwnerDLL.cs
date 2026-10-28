using Dapper;
using Data.EF;
using Domain.DTOs.Owner;
using Microsoft.EntityFrameworkCore;

namespace Data.DLL
{
    public partial class OwnerDLL
    {
        private readonly GamersPlatDbContext _db;

        public OwnerDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        public async Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerId, DateOnly? from = null, DateOnly? to = null)
        {
            try
            {
                if (from.HasValue && to.HasValue && from > to)
                    throw new ArgumentException("The start date must be on or before the end date.");

                var dashboard = await _db.Users
                    .AsNoTracking()
                    .Where(u => u.UserId == ownerId)
                    .Select(u => new DTO_OwnerDashboard
                    {
                        FullName = u.FullName,

                        TotalCenters = (short)_db.Centers
                            .Count(c => c.OwnerUserId == ownerId),

                        ActiveCenters = (short)_db.Centers
                            .Count(c => c.OwnerUserId == ownerId && c.IsActive == true),

                        TotalBookings = _db.Bookings
                            .Count(b => b.Center.OwnerUserId == ownerId),

                        UpcomingBookings = _db.Bookings
                            .Count(b => b.Center.OwnerUserId == ownerId
                                     && (b.Status.ToLower() == "pending" || b.Status.ToLower() == "accepted" || b.Status.ToLower() == "upcoming")),

                        CompletedBookings = _db.Bookings
                            .Count(b => b.Center.OwnerUserId == ownerId
                                     && b.Status.ToLower() == "completed"),

                        CancelledBookings = _db.Bookings
                            .Count(b => b.Center.OwnerUserId == ownerId
                                     && b.Status.ToLower() == "cancelled"),

                        TotalRevenue = _db.Bookings
                            .Where(b => b.Center.OwnerUserId == ownerId
                                     && b.Status.ToLower() == "completed")
                            .Sum(b => (decimal?)b.TotalPrice) ?? 0m,

                        AverageRating = _db.Reviews
                            .Where(r => r.Center.OwnerUserId == ownerId)
                            .Average(r => (decimal?)r.Rating) ?? 0m,

                        TotalReviews = _db.Reviews
                            .Count(r => r.Center.OwnerUserId == ownerId),

                        ActiveServices = (short)_db.Services
                            .Count(s => s.Center.OwnerUserId == ownerId
                                     && s.IsActive),

                        ActiveResources = (short)_db.ResourcesTypes
                            .Count(r => r.Center.OwnerUserId == ownerId
                                     && r.IsActive),

                        ActiveOffers = (short)_db.Offers
                            .Count(o => o.Center.OwnerUserId == ownerId
                                     && o.IsActive),

                        ActiveTournaments = (short)_db.Tournaments
                            .Count(t => t.Center.OwnerUserId == ownerId
                                     && t.StartDate > DateTime.Now)
                    })
                    .FirstOrDefaultAsync();

                if (dashboard == null)
                    return null;

                var endDate = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
                var startDate = from ?? endDate.AddDays(-29);

                dashboard.CenterStatusCounts = await _db.Centers
                    .AsNoTracking()
                    .Where(c => c.OwnerUserId == ownerId)
                    .GroupBy(c => c.CenterStatus ?? "Unknown")
                    .Select(group => new { Status = group.Key, Count = group.Count() })
                    .ToDictionaryAsync(group => group.Status, group => group.Count);

                dashboard.Activity = await _db.Bookings
                    .AsNoTracking()
                    .Where(b => b.Center.OwnerUserId == ownerId && b.BookingDate >= startDate && b.BookingDate <= endDate)
                    .GroupBy(b => b.BookingDate)
                    .Select(group => new DTO_OwnerActivityPoint
                    {
                        Date = group.Key,
                        BookingCount = group.Count(),
                        Revenue = group.Where(b => b.Status.ToLower() == "completed").Sum(b => (decimal?)b.TotalPrice) ?? 0m
                    })
                    .OrderBy(point => point.Date)
                    .ToListAsync();

                return dashboard;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Owner Dashboard Error",
                    ex);

                return null;
            }
        }
    }
}