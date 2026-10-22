using Bussiness.Interfaces;
using Domain.DTOs.Auth;
using Domain.DTOs.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Bussiness.Helpers;

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
        public readonly record struct BookingCreateResult(int StatusCode, string? Message = null, int BookingId = 0)
        {
            public bool IsSuccess => StatusCode is >= 200 and < 300;

            public static BookingCreateResult Created(int id) => new(StatusCodes.Status201Created, null, id);
            public static BookingCreateResult BadRequest(string msg) => new(StatusCodes.Status400BadRequest, msg);
            public static BookingCreateResult Conflict(string msg) => new(StatusCodes.Status409Conflict, msg);
            public static BookingCreateResult ServerError(string msg) => new(StatusCodes.Status500InternalServerError, msg);
        }


        [Authorize]
        [HttpPost]
        [EnableRateLimiting("user-booking")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] DTO_AddBooking dto)
        {
            if (dto == null) return BadRequest();
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            return await CreateInternal(dto, userId);
        }

        [HttpPost("guest")]
        [EnableRateLimiting("guest-booking")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGuest([FromBody] DTO_AddGuestBooking dto)
        {
            var validationError = ValidateRegisterDto.Validate(dto);
            if (validationError != null)
                return BadRequest(new { message = validationError });

            var result = await _booking.CreateGuestBookingAsync(dto);

            if (result.Failure == GuestBookingFailure.ExistingAccount)
                return Conflict(new { message = result.Message });

            if (result.Failure == GuestBookingFailure.RegistrationFailed)
                return StatusCode(500, new { message = result.Message });

            if (!result.Success)
            {
                var statusCode = result.Message?.Contains("availability", StringComparison.OrdinalIgnoreCase) == true ||
                                 result.Message?.Contains("Insufficient", StringComparison.OrdinalIgnoreCase) == true
                    ? StatusCodes.Status409Conflict
                    : result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true
                        ? StatusCodes.Status404NotFound
                        : StatusCodes.Status400BadRequest;

                return StatusCode(statusCode, new { message = result.Message, token = (object?)null, accountCreated = false });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.BookingId }, new { id = result.BookingId, token = result.Token });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("admin")]
        [EnableRateLimiting("user-booking")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateByAdmin([FromBody] DTO_AdminAddBooking dto)
        {
            if (dto == null) 
                return BadRequest();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

                if (string.IsNullOrWhiteSpace(dto.CustomerName) || string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    return BadRequest(new { message = "credentials required" });

            return await CreateInternal(dto, userId, dto.CustomerName, dto.PhoneNumber);
        }

        [Authorize]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] DTO_UpdateBooking dto)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userid)) return Unauthorized();

            var ok = await _booking.Update(dto, userid);
            if (!ok)
            {
                var err = _booking.LastError ?? "Failed to update booking";

                if (err.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { message = err });

                if (err.Contains("not authorized", StringComparison.OrdinalIgnoreCase))
                    return Forbid();

                if (err.Contains("not available", StringComparison.OrdinalIgnoreCase) ||
                    err.Contains("quantity", StringComparison.OrdinalIgnoreCase) ||
                    err.Contains("conflict", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { message = err });

                if (err.Contains("Only pending or accepted", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { message = err });

                if (!string.IsNullOrWhiteSpace(_booking.LastError))
                    return BadRequest(new { message = err });

                return StatusCode(StatusCodes.Status500InternalServerError);
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
        [HttpPut("cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(DTO_CancelBooking dto)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userid)) return Unauthorized();

            var ok = await _booking.Cancel(dto, userid);
            if (!ok)
            {
                var err = _booking.LastError ?? "Failed to cancel booking";

                if (err.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { message = err });

                if (err.Contains("not authorized", StringComparison.OrdinalIgnoreCase))
                    return Forbid();

                if (err.Contains("cannot be cancelled", StringComparison.OrdinalIgnoreCase) ||
                    err.Contains("already cancelled", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { message = err });

                if (!string.IsNullOrWhiteSpace(_booking.LastError))
                    return BadRequest(new { message = err });

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _booking.GetByID(id);
            if (booking == null) return NotFound();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            // Allow access to the booking owner or admin/staff only
            if (booking.UserId.HasValue)
            {
                if (booking.UserId.Value != uid && !User.IsInRole("Admin") && !User.IsInRole("Staff")) return Forbid();
            }

            return Ok(booking);
        }

        [HttpGet("fully-booked-hours")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFullyBookedHours([FromQuery] int centerId,[FromQuery] int resourceTypeId,[FromQuery] DateOnly date)
        {
            List<DTO_StartEndTime>? fullTimeList = await _booking.GetFullBookedHours(centerId, resourceTypeId, date);
            return Ok(fullTimeList);
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




        // Internal method to handle booking creation logic
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        private async Task<IActionResult> CreateInternal(DTO_BookingBase dto, int bookingUserId, string? customerName = null, string? phone = null)
        {
            var ok = await _booking.Add(dto, bookingUserId, customerName, phone);
            if (!ok)
            {
                var err = _booking.LastError;
                if (string.IsNullOrWhiteSpace(err)) return StatusCode(500);
                if (err.Contains("availability", StringComparison.OrdinalIgnoreCase) ||
                    err.Contains("Insufficient", StringComparison.OrdinalIgnoreCase)) return Conflict(new { message = err });
                if (err.Contains("not found", StringComparison.OrdinalIgnoreCase)) return NotFound(new { message = err });
                return BadRequest(new { message = err });
            }
            return CreatedAtAction(nameof(GetById), new { id = _booking.LastId }, new { id = _booking.LastId });
        }
    }
}
