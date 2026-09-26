using Bussiness.Interfaces;
using Domain.DTOs.Booking;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _booking;
        private readonly IResourcesTypeService _resourcesTypeService;
        private readonly IOwnerService _ownerService;

        public BookingsController(IBookingService booking, IResourcesTypeService resourcesTypeService, IOwnerService ownerService)
        {
            _booking = booking ?? throw new ArgumentNullException(nameof(booking));
            _resourcesTypeService = resourcesTypeService ?? throw new ArgumentNullException(nameof(resourcesTypeService));
            _ownerService = ownerService ?? throw new ArgumentNullException(nameof(ownerService));
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Create([FromBody] DTO_AddBooking dto)
        {
            if (dto == null) return BadRequest();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();
            dto.UserId = userId;

            var rt = await _resourcesTypeService.GetByID(dto.ResourcesTypeId);
            if (rt == null) return BadRequest(new { message = "ResourcesType not found" });

            var ok = await _booking.Add(dto);
            if (!ok)
            {
                var err = _booking.LastError;
                if (!string.IsNullOrWhiteSpace(err))
                {
                    if (err.Contains("availability", StringComparison.OrdinalIgnoreCase) || err.Contains("Insufficient", StringComparison.OrdinalIgnoreCase))
                        return Conflict(new { message = err });
                    if (err.Contains("not found", StringComparison.OrdinalIgnoreCase))
                        return NotFound(new { message = err });
                    return BadRequest(new { message = err });
                }
                return StatusCode(500);
            }

            return CreatedAtAction(nameof(GetById), new { id = _booking.LastId }, dto);
        }

        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckAvailability([FromQuery] int centerId, [FromQuery] int resourcesTypeId, [FromQuery] DateTime date, [FromQuery] int? quantity)
        {
            try
            {
                var dateOnly = DateOnly.FromDateTime(date);

                var resource = await _resourcesTypeService.GetByID(resourcesTypeId);
                if (resource == null || resource.CenterId != centerId) return NotFound(new { message = "Resource type not found" });

                // Use booking service to compute booked quantity (service should encapsulate data access)
                var booked = await _booking.GetBookedQuantity(centerId, resourcesTypeId, dateOnly);
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

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _booking.GetByID(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] DTO_UpdateBooking dto)
        {
            if (dto == null || dto.BookingId != id) return BadRequest();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            // fetch existing booking to validate ownership
            var existing = await _booking.GetByID(id);
            if (existing == null) return NotFound();

            // allow only booking owner or admin
            if (existing.UserId.HasValue)
            {
                if (existing.UserId.Value != uid && !User.IsInRole("Admin")) return Forbid();
            }

            var ok = await _booking.Update(dto);
            if (!ok)
            {
                var err = _booking.LastError;
                if (!string.IsNullOrWhiteSpace(err))
                {
                    if (err.Contains("not found", StringComparison.OrdinalIgnoreCase)) return NotFound(new { message = err });
                    return BadRequest(new { message = err });
                }
                return StatusCode(500);
            }

            return NoContent();
        }

        [Authorize]
        [HttpPut("cancel/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var existing = await _booking.GetByID(id);
            if (existing == null) return NotFound();

            // Only booking owner, owner of center, or admin can cancel
            if (existing.UserId.HasValue && existing.UserId.Value == uid)
            {
                // allowed
            }
            else if (User.IsInRole("Owner"))
            {
                // owner: ensure they own the center
                var center = await _resourcesTypeService.GetByID(existing.ResourcesTypeId);
                if (center == null) return NotFound();
                if (center.CenterId != existing.CenterId) return Forbid();
            }
            else if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var ok = await _booking.Cancel(id);
            if (!ok)
            {
                var err = _booking.LastError;
                if (!string.IsNullOrWhiteSpace(err)) return BadRequest(new { message = err });
                return StatusCode(500);
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _booking.GetByID(id);
            if (existing == null) return NotFound();

            var ok = await _booking.Delete(id);
            if (!ok) return StatusCode(500);
            return NoContent();
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMine()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();
            var list = await _booking.GetByPlayerId(uid);
            return Ok(list);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOwnerBookings()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();
            var list = await _booking.GetBookingsByOwnerId(uid);
            return Ok(list);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("owner/accept/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> OwnerAccept(int id)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            try
            {
                var ok = await _ownerService.AcceptBooking(id, uid);
                if (!ok) return StatusCode(500);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("owner/reject/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> OwnerReject(int id)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            try
            {
                var ok = await _ownerService.RejectBooking(id, uid);
                if (!ok) return StatusCode(500);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
