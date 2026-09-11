using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GamersPlatAPI.Controllers.Owner;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        [Authorize(Roles = "Owner")]
        [HttpPost("centers")]
        public IActionResult CreateCenter([FromBody] Data.Center center)
        {
            if (center == null) return BadRequest();
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(id, out var uid)) center.OwnerUserId = uid;
            var bll = new CenterBLL(center);
            if (!bll.Add()) return StatusCode(500);
            return CreatedAtAction("GetCenter", new { id = bll._centerID }, center);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("centers")]
        public async Task<IActionResult> GetMyCenters()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();
            var all = await CenterBLL.GetAll();
            var mine = all.Where(c => c.OwnerUserId == uid).ToList();
            return Ok(mine);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("offers/{centerId:int}")]
        public async Task<IActionResult> GetCenterOffers(int centerId)
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var center = await CenterBLL.GetByID(centerId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();

            var offers = await OfferBLL.GetByCenterId(centerId);
            return Ok(offers);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("offers")]
        public async Task<IActionResult> CreateOffer([FromBody] Data.Offer offer)
        {
            if (offer == null) return BadRequest();
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var center = await CenterBLL.GetByID(offer.CenterId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();

            var bll = new OfferBLL(offer);
            if (!bll.Add()) return StatusCode(500);
            return CreatedAtAction(nameof(GetCenterOffers), new { centerId = offer.CenterId }, offer);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("offers/{id:int}")]
        public async Task<IActionResult> UpdateOffer(int id, [FromBody] Data.Offer offer)
        {
            if (offer == null || id != offer.OfferId) return BadRequest();
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var existing = await OfferBLL.GetByID(id);
            if (existing == null) return NotFound();

            var center = await CenterBLL.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            var bll = new OfferBLL(offer);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("offers/{id:int}")]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            var existing = await OfferBLL.GetByID(id);
            if (existing == null) return NotFound();
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await CenterBLL.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            var bll = new OfferBLL();
            if (!bll.Delete(id)) return StatusCode(500);
            return NoContent();
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("bookings")]
        public async Task<IActionResult> GetOwnerBookings()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var centers = (await CenterBLL.GetAll()).Where(c => c.OwnerUserId == uid).Select(c => c.CenterId).ToList();
            var allBookings = await BookingBLL.GetAll();
            var mine = allBookings.Where(b => centers.Contains(b.CenterId)).ToList();
            return Ok(mine);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("bookings/status/{id:int}")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Status)) return BadRequest();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var booking = await BookingBLL.GetByID(id);
            if (booking == null) return NotFound();

            // ensure owner owns the center
            var center = await CenterBLL.GetByID(booking.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            booking.Status = req.Status;
            if (req.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)) booking.CancelledAt = DateTime.UtcNow;

            var bll = new BookingBLL(booking);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var centers = (await CenterBLL.GetAll()).Where(c => c.OwnerUserId == uid).ToList();
            var centerIds = centers.Select(c => c.CenterId).ToList();

            var bookings = (await BookingBLL.GetAll()).Where(b => centerIds.Contains(b.CenterId)).ToList();
            var tournaments = (await TournamentBLL.GetAll()).Where(t => centerIds.Contains(t.CenterId)).ToList();

            decimal revenue = bookings.Sum(b => b.TotalPrice);
            int devicesCount = tournaments.Select(t => t.DeviceId).Distinct().Count();

            var dashboard = new GamersPlatAPI.DTOs.OwnerDashboardDTO
            {
                OwnerUserId = uid,
                CentersCount = centers.Count,
                BookingsCount = bookings.Count,
                RevenueTotal = revenue,
                DevicesCount = devicesCount,
                TournamentsCount = tournaments.Count
            };

            return Ok(dashboard);
        }
    }
}
