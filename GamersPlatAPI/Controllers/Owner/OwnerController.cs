using Bussiness;
using Bussiness.Interfaces;
using Data;
using GamersPlatAPI.Controllers.Owner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner")]
    public class OwnerController : ControllerBase
    {
        private readonly ICenterService _center;
        private readonly IOfferService _offer;

        public OwnerController(ICenterService center, IOfferService offer)
        {
            _center = center;
            _offer = offer;
        }
        
        [HttpPost("centers")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCenter([FromBody] Data.Center center)
        {
            if (center == null) return BadRequest();
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(id, out var uid)) center.OwnerUserId = uid;
            var bll = new CenterBLL();
            if (!await bll.Add(center)) return StatusCode(500);
            return CreatedAtAction("GetCenter", new { id = bll._centerID }, center);
        }

        [HttpGet("centers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyCenters()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();
            var all = await new _center.GetAll();
            var mine = all.Where(c => c.OwnerUserId == uid).ToList();
            return Ok(mine);
        }

        [HttpGet("offers/{centerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCenterOffers(int centerId)
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var center = await _center.GetByID(centerId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();
            var offers = await _offer.GetByCenterId(centerId);
            return Ok(offers);
        }

        [HttpPost("offers")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOffer([FromBody] Data.Offer offer)
        {
            if (offer == null) return BadRequest();
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var center = await _center.GetByID(offer.CenterId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();
            if (!await _offer.Add(offer)) return StatusCode(500);
            return CreatedAtAction(nameof(GetCenterOffers), new { centerId = offer.CenterId }, offer);
        }

        [HttpPut("offers/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOffer(int id, [FromBody] Data.Offer offer)
        {
            if (offer == null || id != offer.OfferId) return BadRequest();
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var existing = await _offer.GetByID(id);
            if (existing == null) return NotFound();

            var center = await _center.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();
            if (!await _offer.Update(offer)) return StatusCode(500);
            return NoContent();
        }

        [HttpDelete("offers/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            var existing = await _offer.GetByID(id);
            if (existing == null) return NotFound();
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await _center.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();
            if (!await _offer.Delete(id)) return StatusCode(500);
            return NoContent();
        }

        [HttpGet("bookings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnerBookings()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var centers = (await new CenterBLL().GetAll()).Where(c => c.OwnerUserId == uid).Select(c => c.CenterId).ToList();
            var allBookings = await new BookingBLL(new ResourcesTypeBLL()).GetAll();
            var mine = allBookings.Where(b => centers.Contains(b.CenterId)).ToList();
            return Ok(mine);
        }

        [HttpPut("bookings/status/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Status)) return BadRequest();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var booking = await new BookingBLL(new ResourcesTypeBLL()).GetByID(id);
            if (booking == null) return NotFound();

            // ensure owner owns the center
            var center = await new CenterBLL().GetByID(booking.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            booking.Status = req.Status;
            if (req.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)) booking.CancelledAt = DateTime.UtcNow;

            var bll = new BookingBLL(new ResourcesTypeBLL());
            if (!await bll.Update(booking)) return StatusCode(500);
            return NoContent();
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Dashboard()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var uid)) return Unauthorized();

            var centers = (await new .GetAll()).Where(c => c.OwnerUserId == uid).ToList();
            var centerIds = centers.Select(c => c.CenterId).ToList();

            var bookings = (await new BookingBLL(new ResourcesTypeBLL()).GetAll()).Where(b => centerIds.Contains(b.CenterId)).ToList();
            var tournaments = (await new TournamentBLL().GetAll()).Where(t => centerIds.Contains(t.CenterId)).ToList();

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
