using Bussiness;
using Bussiness.Interfaces;
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
        public async Task<IActionResult> GetProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var user = await _user.GetById(userId);
            if (user == null) return NotFound();

            var dto = new DTOs.UserProfileDTO
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Points = user.Points,
                IsActive = user.IsActive,
                CityId = user.CityId,
                CreatedAt = user.CreatedAt,
                EmailVerified = user.EmailVerified ?? false,
                PhoneVerified = user.PhoneVerified ?? false
            };

            return Ok(dto);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] Data.User model)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            if (model == null || model.UserId != userId) return BadRequest();

            if (!await _user.Update(userId, model)) return StatusCode(500);
            return NoContent();
        }

        [HttpGet("bookings")]
        [Authorize]
        public async Task<IActionResult> GetBookings()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var bookings = await _booking.GetByPlayerId(userId);
            return Ok(bookings);
        }
    }
}
