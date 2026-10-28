using Bussiness;
using Bussiness.BLL;
using Bussiness.Interfaces;
using Domain.DTOs.Auth;
using Domain.DTOs.Center;
using Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        readonly IUser _user;
        readonly CenterBLL _center;
        readonly BookingBLL _booking;
        readonly AdminBLL _admin;
        private readonly IAuthService _authService;

        public AdminController(IUser user, CenterBLL center, BookingBLL booking, AdminBLL admin, IAuthService authService)
        {
            _user = user;
            _center = center;
            _booking = booking;
            _admin = admin;
            _authService = authService;
        }


        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var users = await _admin.GetAdminDashboardStatistics();
                return Ok(new { users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to retrieve dashboard statistics. Please try again." + ex.Message);
            }
        }


        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _user.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to retrieve users. Please try again." + ex.Message);
            }
        }

        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUser([FromBody] UserRegisterRequest request)
        {
            if (request == null)
                return BadRequest("Invalid user data");
            try
            {
                await _authService.RegisterWithRoleAsync(request, request.Role ?? "player");
                return Ok(new { message = "User added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to add user. Please try again." + ex.Message);
            }
        }

        [HttpPut("user/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUser([FromBody] DTO_UpdateUserInfoRequest dto, [FromRoute] int userId)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var adminId)) return Unauthorized();

            if (dto == null) return BadRequest(new { message = "Invalid request" });

            var ok = await _user.Update(userId, dto, adminId);
            if (!ok)
            {
                var err = _user.LastError ?? "Failed to update user";

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

        [HttpPut("user/{userId:int}/block")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> BlockUser([FromRoute] int userId)
        {
            try
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idClaim, out var adminId)) return Unauthorized();

                bool IsBlocked = await _admin.BlockUser(userId, adminId);
                if (IsBlocked)
                    return Ok();
            }
            catch(Exception)
            {
                return BadRequest(new { message = "Error While Blocking User" });
            }
            return NoContent();
        }

        [HttpPut("user/{userId:int}/unblock")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UnblockUser([FromRoute] int userId)
        {
            try
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idClaim, out var adminId)) return Unauthorized();

                bool IsBlocked = await _admin.UnblockUser(userId, adminId);
                if (IsBlocked)
                    return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error While UnBlocking User" });
            }
            return NoContent();
        }



        [HttpGet("centers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCenters()
        {
            var centers = await _center.GetAll();
            return Ok(centers);
        }

        [HttpPost("center")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCenter([FromBody] DTO_CreateCenter addCenter)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            bool IsAdded = await _center.Add(addCenter, userId);
            if (IsAdded)
                return Created();
            return BadRequest(new { message = "Error While Creating Center" });
        }

        [HttpPut("centers/{centerId:int}/approve")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ApproveCenter([FromRoute] int centerId)
        {
            if (! await _admin.ApproveCenter(centerId)) 
                return StatusCode(500, new { message = "Error While Approving Center" });
            return NoContent();
        }

        [HttpPut("centers/{centerId:int}/reject")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RejectCenter([FromRoute] int centerId)
        {
            if (!await _admin.RejectCenter(centerId, "Center Rejected, Call Us To Full Reason")) 
                return StatusCode(500, new { message = "Error While Rejecting Center" });
            return NoContent();
        }

        [HttpPut("centers/{centerId:int}/active")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ActiveCenter([FromRoute] int centerId)
        {
            var center = await _center.ActiveCenter(centerId);
            if (center == false) return StatusCode(500, new { message = "Error While Activating Center" });

            return NoContent();
        }

        [HttpPut("centers/{centerId:int}/inactive")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> InactiveCenter([FromRoute] int centerId)
        {
            var center = await _center.DeactiveCenter(centerId);
            if (center == false) return StatusCode(500, new { message = "Error While Deactivating Center" });
            return NoContent();
        }



        [HttpGet("bookings")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _booking.GetAll();
            if (bookings == null) return NotFound();
            return Ok(bookings);
        }



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

        [HttpPut("profile/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest dto)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            if (dto == null) return BadRequest(new { message = "Invalid request" });

            var result = await _authService.ChangePassword(userId, dto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
