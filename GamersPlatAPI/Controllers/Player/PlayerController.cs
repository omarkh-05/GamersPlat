using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var user = await UserBLL.GetByID(userId);
            if (user == null) return NotFound();

            var dto = new GamersPlatAPI.DTOs.UserProfileDTO
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
        public IActionResult UpdateProfile([FromBody] Data.User model)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            if (model == null || model.UserId != userId) return BadRequest();

            var bll = new UserBLL(model);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [HttpGet("bookings")]
        [Authorize]
        public async Task<IActionResult> GetBookings()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var bookings = await BookingBLL.GetByUserId(userId);
            return Ok(bookings);
        }
    }
}
