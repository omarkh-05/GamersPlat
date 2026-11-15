using Bussiness.BLL;
using Bussiness.Interfaces;
using Data;
using Domain.DTOs.Auth;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "staff")]
    public class StaffController : ControllerBase
    {
        private readonly StaffBLL _staff;
        public StaffController(StaffBLL staff)
        {
            _staff = staff;
        }

        // Dashboard Overview
       /* [HttpGet("overview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDashboardOverview()
        {
            if (!TryGetOwnerId(out var ownerId))
                return Unauthorized();

            return await Run(() => _staff.GetStaffDashboard(ownerId), res => Ok(res));
        }*/
        // Dashboard Overview


        // Staff Profile
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaffProfile()
        {
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.GetStaffMemberById(staffId), result => Ok(result));
        }
        [HttpPut("profile/password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangeStaffPassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var userId)) return Unauthorized();
            if (request == null) return BadRequest(new { message = "Invalid request" });

            return await Run(() => _staff.ChangeStaffPassword(userId, request), result => Ok(result));
        }
        // Staff Profile


        // Center Resources
        [HttpGet("centers/{centerId:int}/resources")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCenterResources([FromRoute] int centerId)
        {
            if (!TryGetOwnerId(out var userId)) return Unauthorized();
            return await Run(() => _staff.GetAllResourceByCenterId(centerId, userId), result => Ok(result));
        }
        [HttpPost("centers/{centerId:int}/resources")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddResourceToCenter(int centerId, [FromBody] DTO_AddResource request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var userId)) return Unauthorized();
            return await Run(() => _staff.AddResourceToCenter(request, centerId, userId), result => CreatedAtAction(nameof(GetCenterResources), new { centerId }, result));
        }
        [HttpPut("centers/{centerId:int}/resources/{centerResourceId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCenterResource([FromRoute] int centerId, [FromRoute] int centerResourceId, [FromBody] DTO_UpdateResource request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var userId)) return Unauthorized();
            return await Run(() => _staff.UpdateCenterResource(request, centerId, centerResourceId, userId), result => Ok(result));
        }
        [HttpPatch("centers/{centerId:int}/resources/{centerResourceId:int}/active")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActiveCenterResource(int centerId, int centerResourceId)
        {
            if (!TryGetOwnerId(out var userId)) return Unauthorized();
            return await Run(() => _staff.ActiveCenterResource(centerId, centerResourceId, userId), _ => NoContent());
        }
        [HttpPatch("centers/{centerId:int}/resources/{centerResourceId:int}/deactive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactiveCenterResource(int centerId, int centerResourceId)
        {
            if (!TryGetOwnerId(out var userId)) return Unauthorized();
            return await Run(() => _staff.DeactiveCenterResource(centerId, centerResourceId, userId), _ => NoContent());
        }
        // Center Resources

        // Center Services
        [HttpGet("centers/{centerId:int}/services")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCenterServices(int centerId)
        {
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.GetCenterServices(centerId, staffId), result => Ok(result));
        }
        [HttpPost("centers/{centerId:int}/services")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddServiceToCenter([FromRoute] int centerId, [FromBody] DTO_AddCenterService request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.AddCenterService(request, centerId, staffId), result => CreatedAtAction(nameof(GetCenterServices), new { centerId }, result));
        }
        [HttpPut("centers/{centerId:int}/services/{centerServiceId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCenterService([FromRoute] int centerId, [FromRoute] int centerServiceId, [FromBody] DTO_UpdateCenterService request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.UpdateCenterService(request, centerId, centerServiceId, staffId), result => Ok(result));
        }
        [HttpPatch("centers/{centerId:int}/services/{centerServiceId:int}/active")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActiveCenterService(int centerId, int centerServiceId)
        {
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.ActiveCenterService(centerId, centerServiceId, staffId), _ => NoContent());
        }
        [HttpPatch("centers/{centerId:int}/services/{centerServiceId:int}/deactive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactiveCenterService(int centerId, int centerServiceId)
        {
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.DeactiveCenterService(centerId, centerServiceId, staffId), _ => NoContent());
        }
        // Center Services
        // Bookings Management
        [HttpGet("bookings/{centerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllBookingsForCenter(int centerId)
        {
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.GetAllBookings_ByCenterId(centerId, staffId), result => Ok(result));
        }
        [HttpPatch("bookings/{bookingId:int}/accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AcceptBookingStatus(int bookingId)
        {
            if (bookingId <= 0) return BadRequest();
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.AcceptBooking(bookingId, staffId), result => Ok(result));
        }
        [HttpPatch("bookings/{bookingId:int}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RejectBookingStatus(int bookingId)
        {
            if (bookingId <= 0) return BadRequest();
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.RejectBooking(bookingId, staffId), result => Ok(result));
        }
        [HttpPatch("bookings/{bookingId:int}/start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> StartBooking(int bookingId)
        {
            if (bookingId <= 0) return BadRequest();
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.StartBooking(bookingId, staffId), result => Ok(result));
        }
        [HttpPatch("bookings/{bookingId:int}/stop")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> StopBooking(int bookingId)
        {
            if (bookingId <= 0) return BadRequest();
            if (!TryGetOwnerId(out var staffId)) return Unauthorized();
            return await Run(() => _staff.StopBooking(bookingId, staffId), result => Ok(result));
        }
        // Center Bookings

        // Helpers
        private bool TryGetOwnerId(out int staffId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out staffId) && staffId > 0;
        }
        private async Task<IActionResult> Run<T>(Func<Task<T>> action, Func<T, IActionResult> success)
        {
            try
            {
                var result = await action();

                if (result is null)
                    return NotFound();

                return success(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
