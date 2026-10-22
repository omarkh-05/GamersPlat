using Bussiness;
using Bussiness.Interfaces;
using Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        readonly IUser _user;
        readonly BookingBLL _booking;

        public PlayerController(IUser user, BookingBLL booking)
        {
            _user = user;
            _booking = booking;
        }

        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var user = await _user.GetById(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateProfile([FromBody] DTO_UpdateUserInfoRequest dto)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            if (dto == null) return BadRequest(new { message = "Invalid request" });

            var ok = await _user.Update(userId, dto);
            if (!ok)
            {
                var err = _user.LastError ?? "Failed to update profile";

                if (err.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { message = err });

                if (err.Contains("conflict", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { message = err });

                if (err.Contains("unexpected", StringComparison.OrdinalIgnoreCase))
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = err });

                return BadRequest(new { message = err });
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("bookings")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookings()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var bookings = await _booking.GetByPlayerId(userId);
            return Ok(bookings);
        }
    }
}
