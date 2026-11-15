using Dapper;
using Data.EF;
using Domain.DTOs.Owner;
using Microsoft.EntityFrameworkCore;

namespace Data.DLL
{
    public class OwnerDLL
    {
        private readonly GamersPlatDbContext _db;

        public OwnerDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        public async Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerId)
        {
            try
            {
                return await _db.Users
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
                                     && b.Status == "pending"),

                        CompletedBookings = _db.Bookings
                            .Count(b => b.Center.OwnerUserId == ownerId
                                     && b.Status == "completed"),

                        CancelledBookings = _db.Bookings
                            .Count(b => b.Center.OwnerUserId == ownerId
                                     && b.Status == "cancelled"),

                        TotalRevenue = _db.Bookings
                            .Where(b => b.Center.OwnerUserId == ownerId && b.Status == "completed")
                            .Sum(b => (decimal?)b.TotalPrice) ?? 0m,

                        AverageRating = _db.Reviews
                            .Where(r => r.Center.OwnerUserId == ownerId)
                            .Average(r => (decimal?)r.Rating) ?? 0m,

                        TotalReviews = _db.Reviews
                            .Count(r => r.Center.OwnerUserId == ownerId),

                        TotalEmployees = _db.Staff
                            .Count(e => e.Center.OwnerUserId == ownerId),

                        ActiveServices = (short)_db.CenterServices
                            .Count(s => s.Center.OwnerUserId == ownerId
                                     && s.IsActive),
                      
                        ActiveResources = (short)_db.CenterResources
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