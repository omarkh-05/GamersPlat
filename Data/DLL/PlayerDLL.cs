using Data.EF;
using Domain.DTOs.Player;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data.DLL
{
    public class PlayerDLL
    {
        private readonly GamersPlatDbContext _db;

        public PlayerDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ Read By ================
        public async Task<DTO_PlayerProfile?> GetPlayerProfile(int userId)
        {
            try
            {
                return await _db.Users
                    .AsNoTracking()
                    .Where(u => u.UserId == userId)
                    .Select(u => new DTO_PlayerProfile
                    {
                        // User Information
                        PhoneNumber = u.PhoneNumber,
                        FullName = u.FullName,
                        Email = u.Email,
                        CityName = u.City != null ? u.City.Name : "(choose your city)",
                        Points = u.Points,
                        IsActive = u.IsActive,
                        PhoneVerified = u.PhoneVerified ?? false,
                        EmailVerified = u.EmailVerified ?? false,

                        // Player Statistics
                        TotalBookings = _db.Bookings
                            .Count(b => b.UserId == userId && b.Status == "Completed"),

                        TotalTournaments = _db.TournamentPlayers
                            .Count(tp => tp.UserId == userId),

                        TotalPaid = _db.Bookings
                            .Where(b => b.UserId == userId && b.Status == "Completed")
                            .Sum(b => (decimal?)b.TotalPrice) ?? 0m,

                        MostPlayedCenterId = _db.Bookings
                            .Where(b => b.UserId == userId && b.Status == "Completed")
                            .GroupBy(b => b.CenterId)
                            .Select(g => new
                            {
                                CenterId = g.Key,
                                BookingCount = g.Count()
                            })
                            .OrderByDescending(g => g.BookingCount)
                            .ThenBy(g => g.CenterId)
                            .Select(g => (int?)g.CenterId)
                            .FirstOrDefault(),

                        MostPlayedCenterName = _db.Bookings
                            .Where(b => b.UserId == userId && b.Status == "Completed")
                            .GroupBy(b => b.CenterId)
                            .Select(g => new
                            {
                                CenterId = g.Key,
                                BookingCount = g.Count()
                            })
                            .OrderByDescending(g => g.BookingCount)
                            .ThenBy(g => g.CenterId)
                            .Select(g => g.CenterId)
                            .Join(
                                _db.Centers,
                                centerId => centerId,
                                c => c.CenterId,
                                (centerId, c) => c.CenterName
                            )
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Player Statistics Error",ex);

                return null;
            }
        }
        // ================ Read By ================
    }
}