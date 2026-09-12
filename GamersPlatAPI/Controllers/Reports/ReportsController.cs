using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        [Authorize]
        [HttpGet("player")]
        public async Task<IActionResult> PlayerReports()
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var bookings = await BookingBLL.GetByUserId(uid);
            var totalSpent = bookings.Sum(b => b.TotalPrice);
            var totalPoints = bookings.Sum(b => b.EarnedPoints ?? 0);

            var dto = new GamersPlatAPI.DTOs.PlayerReportDTO
            {
                UserId = uid,
                BookingsCount = bookings.Count,
                TotalSpent = totalSpent,
                TotalPointsEarned = totalPoints
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner")]
        public async Task<IActionResult> OwnerReports()
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var centers = (await CenterBLL.GetAll()).Where(c => c.OwnerUserId == uid).ToList();
            var centerIds = centers.Select(c => c.CenterId).ToList();
            var bookings = (await BookingBLL.GetAll()).Where(b => centerIds.Contains(b.CenterId)).ToList();

            var report = new GamersPlatAPI.DTOs.OwnerReportDTO
            {
                OwnerUserId = uid,
                CentersCount = centers.Count,
                BookingsCount = bookings.Count,
                TotalRevenue = bookings.Sum(b => b.TotalPrice),
                TotalEarnedPoints = bookings.Sum(b => b.EarnedPoints ?? 0)
            };

            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("revenue")]
        public async Task<IActionResult> RevenueReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var all = await BookingBLL.GetAll();
            var filtered = all.AsQueryable();
            if (from.HasValue) filtered = filtered.Where(b => b.CreatedAt >= from.Value);
            if (to.HasValue) filtered = filtered.Where(b => b.CreatedAt <= to.Value);

            var totalRevenue = filtered.Sum(b => b.TotalPrice);
            var byCenter = filtered.GroupBy(b => b.CenterId).Select(g => new { CenterId = g.Key, Revenue = g.Sum(x => x.TotalPrice), Bookings = g.Count() }).ToList();

            return Ok(new { TotalRevenue = totalRevenue, ByCenter = byCenter });
        }
    }
}
