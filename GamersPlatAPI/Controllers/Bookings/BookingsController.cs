using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] Data.Booking booking)
        {
            if (booking == null) return BadRequest();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var userId)) booking.UserId = userId;

            var bll = new BookingBLL(booking);
            if (!bll.Add())
            {
                if (!string.IsNullOrWhiteSpace(bll.LastError))
                {
                    if (bll.LastError.Contains("availability", StringComparison.OrdinalIgnoreCase))
                        return Conflict(new { message = bll.LastError });
                    if (bll.LastError.Contains("not found", StringComparison.OrdinalIgnoreCase))
                        return NotFound(new { message = bll.LastError });
                    return BadRequest(new { message = bll.LastError });
                }

                return StatusCode(500);
            }

            return CreatedAtAction(nameof(GetById), new { id = bll._bookingID }, booking);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var b = await BookingBLL.GetByID(id);
            if (b == null) return NotFound();
            return Ok(b);
        }

        [Authorize]
        [HttpPut("cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var existing = await BookingBLL.GetByID(id);
            if (existing == null) return NotFound();

            existing.Status = "Cancelled";
            var bll = new BookingBLL(existing);
            if (!bll.Update())
            {
                if (!string.IsNullOrWhiteSpace(bll.LastError))
                    return BadRequest(new { message = bll.LastError });
                return StatusCode(500);
            }
            return NoContent();
        }

        [Authorize]
        [HttpPut("postpone/{id:int}")]
        public async Task<IActionResult> Postpone(int id, [FromBody] PostponeBookingRequest req)
        {
            if (req == null) return BadRequest();

            var existing = await BookingBLL.GetByID(id);
            if (existing == null) return NotFound();

            // Only allow user who booked or admin/owner to postpone
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            if (existing.UserId.HasValue && existing.UserId != uid && !User.IsInRole("Admin")) return Forbid();

            // update date
            var newDate = DateOnly.FromDateTime(req.NewDate);
            existing.BookingDate = newDate;

            var bll = new BookingBLL(existing);
            if (!bll.Update())
            {
                if (!string.IsNullOrWhiteSpace(bll.LastError))
                    return Conflict(new { message = bll.LastError });
                return StatusCode(500);
            }

            return NoContent();
        }

        [HttpGet("check-availability")]
        public IActionResult CheckAvailability([FromQuery] int centerId, [FromQuery] int resourcesTypeId, [FromQuery] DateTime date, [FromQuery] int? quantity)
        {
            try
            {
                var dateOnly = DateOnly.FromDateTime(date);
                using var db = new Data.EF.GamersPlatDbContext();

                var resource = db.ResourcesTypes.FirstOrDefault(rt => rt.ResourcesTypeId == resourcesTypeId && rt.CenterId == centerId);
                if (resource == null) return NotFound(new { message = "Resource type not found" });

                // Sum booked quantities for the same date and resource (exclude cancelled)
                var booked = db.Bookings
                    .Where(b => b.CenterId == centerId && b.ResourcesTypeId == resourcesTypeId && b.BookingDate == dateOnly && b.Status != "Cancelled")
                    .Sum(b => (int?)b.Quantity) ?? 0;

                var available = resource.TotalQuantity - booked;

                var result = new
                {
                    CenterId = centerId,
                    ResourcesTypeId = resourcesTypeId,
                    Date = dateOnly,
                    Available = available,
                    RequestedQuantity = quantity ?? 0,
                    CanFulfill = quantity.HasValue ? available >= quantity.Value : (bool?)null
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error checking availability", detail = ex.Message });
            }
        }
    }
}
