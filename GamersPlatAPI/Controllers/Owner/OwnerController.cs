using Bussiness.Interfaces;
using Data;
using Domain.DTOs.Auth;
using Domain.DTOs.Center;
using Domain.DTOs.Invoice;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Domain.DTOs.Staff;
using Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "owner")]
    public class OwnerController : ControllerBase
    {
        private readonly ICenterService _center;
        private readonly IOwnerService _owner;
        public OwnerController(ICenterService center, IOwnerService owner)
        {
            _center = center;
            _owner = owner;
        }

        // Dashboard Overview
        [HttpGet("overview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDashboardOverview()
        {
            if (!TryGetOwnerId(out var ownerId))
                return Unauthorized();

            return await Run(() => _owner.GetOwnerDashboard(ownerId),res => Ok(res));
        }
        // Dashboard Overview


        // Center Managament
        [HttpGet("centers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetOwnerCenters()
        {
            if (!TryGetOwnerId(out var ownerId))
                return Unauthorized();
            return await Run(() => _owner.GetAllCentersForOwner(ownerId), res => Ok(res));
        }
        [HttpPost("center")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCenter([FromBody] DTO_CreateCenter addCenter)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _center.Add(addCenter, ownerId), result => Created());
        }
        [HttpPut("centers/{centerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCenter(int centerId, [FromBody] DTO_UpdateCenter request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.UpdateCenter(request, centerId,ownerId), result => Ok(result));
        }
        [HttpPatch("centers/{centerId:int}/active")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ActiveCenter([FromRoute] int centerId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.ActiveCenter(centerId, ownerId), result => Ok());
        }
        [HttpPatch("centers/{centerId:int}/inactive")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> InactiveCenter([FromRoute] int centerId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.DeactiveCenter(centerId, ownerId), result => Ok());
        }
        // Center Managament


        // Center Workspace
        [HttpGet("centers/{centerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCenterOverView(int centerId)
        {
            return await Run(() => _owner.GetCenterOverView(centerId), res => Ok(res));
        }
        [HttpGet("centers/{centerId:int}/invoices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCenterInvoices(int centerId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetAllCenterInvoices(centerId,ownerId), res => Ok(res));
        }
        // Center Resources
        [HttpGet("centers/{centerId:int}/resources")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCenterResources([FromRoute] int centerId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetAllResourceByCenterId(centerId, ownerId), result => Ok(result));
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
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.AddResourceToCenter(request,centerId, ownerId), result => CreatedAtAction(nameof(GetCenterResources), new { centerId }, result));
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
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.UpdateCenterResource(request, centerId, centerResourceId, ownerId), result => Ok(result));
        }
        [HttpPatch("centers/{centerId:int}/resources/{centerResourceId:int}/active")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActiveCenterResource(int centerId, int centerResourceId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.ActiveCenterResource(centerId, centerResourceId, ownerId), _ => NoContent());
        }
        [HttpPatch("centers/{centerId:int}/resources/{centerResourceId:int}/deactive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactiveCenterResource(int centerId, int centerResourceId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.DeactiveCenterResource(centerId, centerResourceId, ownerId), _ => NoContent());
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
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetCenterServices(centerId, ownerId), result => Ok(result));
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
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.AddCenterService(request, centerId, ownerId), result => CreatedAtAction(nameof(GetCenterServices), new { centerId }, result));
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
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.UpdateCenterService(request, centerId, centerServiceId, ownerId), result => Ok(result));
        }
        [HttpPatch("centers/{centerId:int}/services/{centerServiceId:int}/active")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActiveCenterService(int centerId, int centerServiceId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.ActiveCenterService(centerId, centerServiceId, ownerId), _ => NoContent());
        }
        [HttpPatch("centers/{centerId:int}/services/{centerServiceId:int}/deactive")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactiveCenterService(int centerId, int centerServiceId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.DeactiveCenterService(centerId, centerServiceId, ownerId), _ => NoContent());
        }
            // Center Services
        [HttpGet("bookings/{centerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllBookingsForCenter(int centerId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetAllUBookings_ByCenterId(centerId, ownerId), result => Ok(result));
        }
        // Center Workspace


        // Bookings Management
        [HttpGet("bookings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllOwnerBookings()
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetAllBookings(ownerId), result => Ok(result));
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
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.AcceptBooking(bookingId, ownerId), result => Ok(result));
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
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.RejectBooking(bookingId, ownerId), result => Ok(result));
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
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.StartBooking(bookingId, ownerId), result => Ok(result));
        }
        [HttpGet("bookings/{bookingId:int}/invoice-preview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvoicePreview(int bookingId)
        {
            if (bookingId <= 0) return BadRequest();
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetInvoicePreview(bookingId, ownerId), result => Ok(result));
        }
        [HttpPatch("bookings/{bookingId:int}/stop")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> StopBooking(int bookingId, [FromBody] DTO_EndBookingInvoice request)
        {
            if (bookingId <= 0) return BadRequest();
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.StopBooking(bookingId, ownerId, request), result => Ok(result));
        }
        // Bookings Management


        // Staff Management
        [HttpGet("staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllOwnerStaff()
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetAllStaffByOwnerId(ownerId), result => Ok(result));
        }
        [HttpGet("staff/{centerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllCenterStaff([FromRoute] int centerId)
        {
            if (centerId <= 0) return BadRequest();
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetAllStaffByCenterId(centerId), result => Ok(result));
        }
        [HttpPost("staff")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateStaff([FromBody] DTO_AddStaffMember request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.AddStaffMember(request), result => CreatedAtAction(nameof(GetAllCenterStaff), result));
        }
        [HttpPut("staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateOwnerStaff([FromBody] DTO_UpdateStaffMember request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.UpdateStaffMember(request), result => Ok(result));
        }
        // Staff Management


        // Owner Profile
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnerProfile()
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.GetOwnerInfo(ownerId), result => Ok(result));
        }
        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateOwnerProfile([FromBody] DTO_UpdateUserInfoRequest request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(async () =>
            {
                await _owner.UpdateOwnerInfo(ownerId, request);
                return await _owner.GetOwnerInfo(ownerId);
            }, result => Ok(result));
        }
        [HttpPut("profile/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangeOwnerPassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null) return BadRequest();
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await Run(() => _owner.ChangeOwnerPassword(ownerId, request), _ => NoContent());
        }
        // Owner Profile


        // Helpers
        private bool TryGetOwnerId(out int ownerId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out ownerId) && ownerId > 0;
        }
        private async Task<IActionResult> Run<T>(Func<Task<T>> action,Func<T, IActionResult> success)
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
                return StatusCode(500, new {message = "An unexpected error occurred."});
            }
        }
        
        /*[HttpGet("requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public Task<IActionResult> GetOwnerRequests([FromQuery] string? type, [FromQuery] string? status)
        {
            if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
            return Run(() => _owner.GetOwnerRequests(ownerId, type, status), result => Ok(result));
        }

        [HttpGet("requests/{requestId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> GetOwnerRequest(int requestId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
            return Run(() => _owner.GetOwnerRequest(requestId, ownerId), result => Ok(result));
        }

        [HttpPatch("requests/{requestId:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public Task<IActionResult> PatchRequestStatus(int requestId, [FromBody] DTO_OwnerRequestStatusUpdate request)
        {
            if (request == null) return Task.FromResult<IActionResult>(BadRequest());
            if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
            return Run(() => _owner.UpdateOwnerRequestStatus(requestId, request, ownerId), result => Ok(result));
        }*/
    }
}
