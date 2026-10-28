using Bussiness.Interfaces;
using Data;
using Domain.DTOs.Auth;
using Domain.DTOs.Booking;
using Domain.DTOs.Center;
using Domain.DTOs.Owner;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Owner")]
public class OwnerController : ControllerBase
{
    private readonly IOwnerService _owner;
    private readonly ICenterService _center;
    private readonly IOfferService _offer;
    private readonly ILogger<OwnerController> _logger;

    public OwnerController(IOwnerService owner, ICenterService center, IOfferService offer, ILogger<OwnerController> logger)
    {
        _owner = owner;
        _center = center;
        _offer = offer;
        _logger = logger;
    }

    [HttpGet("dashboard/overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> GetDashboardOverview([FromQuery] DTO_OwnerOverviewQuery query)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerDashboard(ownerId, query.From, query.To), result => Ok(result));
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> GetDashboardLegacy([FromQuery] DTO_OwnerOverviewQuery query) => GetDashboardOverview(query);

    [HttpGet("centers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> GetMyCenters()
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetAllCentersForOwner(ownerId), result => Ok(result));
    }

    [HttpGet("centers/{centerId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetOwnerCenter(int centerId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerCenterDetails(centerId, ownerId), result => Ok(result));
    }

    [HttpPost("centers")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> CreateCenter([FromBody] DTO_CreateCenter request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.CreateOwnerCenter(request, ownerId), result => CreatedAtAction(nameof(GetOwnerCenter), new { centerId = result.CenterId }, result));
    }

    [HttpPut("centers/{centerId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateCenter(int centerId, [FromBody] DTO_UpdateCenter request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.UpdateOwnerCenter(centerId, request, ownerId), result => Ok(result));
    }

    [HttpDelete("centers/{centerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteCenter(int centerId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.DeleteOwnerCenter(centerId, ownerId), _ => NoContent());
    }

    [HttpGet("centers/{centerId:int}/workspace")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetCenterWorkspace(int centerId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerWorkspace(centerId, ownerId), result => Ok(result));
    }

    [HttpGet("centers/{centerId:int}/resources")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetOwnerResources(int centerId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerResources(centerId, ownerId), result => Ok(result));
    }

    [HttpPost("centers/{centerId:int}/resources")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> CreateResource(int centerId, [FromBody] DTO_AddResource request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.CreateOwnerResource(centerId, request, ownerId), result => CreatedAtAction(nameof(GetOwnerResources), new { centerId }, result));
    }

    [HttpPut("centers/{centerId:int}/resources/{resourceId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateResource(int centerId, int resourceId, [FromBody] DTO_UpdateResource request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.UpdateOwnerResource(centerId, resourceId, request, ownerId), result => Ok(result));
    }

    [HttpDelete("centers/{centerId:int}/resources/{resourceId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteResource(int centerId, int resourceId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.DeleteOwnerResource(centerId, resourceId, ownerId), _ => NoContent());
    }

    [HttpPut("centers/{centerId:int}/resources/{resourceId:int}/units")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateResourceUnits(int centerId, int resourceId, [FromBody] DTO_OwnerResourceUnitsUpdate request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.UpdateOwnerResourceUnits(centerId, resourceId, request, ownerId), result => Ok(result));
    }

    [HttpGet("centers/{centerId:int}/services")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetOwnerServices(int centerId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerServices(centerId, ownerId), result => Ok(result));
    }

    [HttpPost("centers/{centerId:int}/services")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> CreateService(int centerId, [FromBody] DTO_AddService request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.CreateOwnerService(centerId, request, ownerId), result => CreatedAtAction(nameof(GetOwnerServices), new { centerId }, result));
    }

    [HttpPut("centers/{centerId:int}/services/{serviceId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateService(int centerId, int serviceId, [FromBody] DTO_UpdateService request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.UpdateOwnerService(centerId, serviceId, request, ownerId), result => Ok(result));
    }

    [HttpDelete("centers/{centerId:int}/services/{serviceId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteService(int centerId, int serviceId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.DeleteOwnerService(centerId, serviceId, ownerId), _ => NoContent());
    }

    [HttpGet("bookings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> GetOwnerBookings([FromQuery] DTO_OwnerBookingsQuery query)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerBookings(ownerId, query), result => Ok(result));
    }

    [HttpGet("bookings/{bookingId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetOwnerBooking(int bookingId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerBooking(bookingId, ownerId), result => Ok(result));
    }

    [HttpPatch("bookings/{bookingId:int}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> PatchBookingStatus(int bookingId, [FromBody] DTO_OwnerBookingStatusUpdate request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.UpdateOwnerBookingStatus(bookingId, request, ownerId), result => Ok(result));
    }

    [HttpPut("bookings/status/{bookingId:int}")]
    public Task<IActionResult> UpdateBookingStatusLegacy(int bookingId, [FromBody] GamersPlatAPI.Controllers.Owner.UpdateBookingStatusRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Status)) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        if (request.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            return Run(() => _owner.RejectBooking(bookingId, ownerId), _ => NoContent());
        return Run(
            () => _owner.UpdateOwnerBookingStatus(bookingId, new DTO_OwnerBookingStatusUpdate { Status = request.Status }, ownerId),
            _ => NoContent());
    }

    [HttpGet("requests")]
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
    }

    [HttpGet("employees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> GetOwnerEmployees([FromQuery] int? centerId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerEmployees(ownerId, centerId), result => Ok(result));
    }

    [HttpPost("employees")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> CreateOwnerEmployee([FromBody] DTO_OwnerEmployeeCreateRequest request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.CreateOwnerEmployee(request, ownerId), result => CreatedAtAction(nameof(GetOwnerEmployees), new { centerId = result.CenterId }, result));
    }

    [HttpPut("employees/{employeeId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> UpdateOwnerEmployee(int employeeId, [FromBody] DTO_OwnerEmployeeUpdateRequest request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.UpdateOwnerEmployee(employeeId, request, ownerId), result => Ok(result));
    }

    [HttpDelete("employees/{employeeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteOwnerEmployee(int employeeId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.DeleteOwnerEmployee(employeeId, ownerId), _ => NoContent());
    }

    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetOwnerProfile()
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.GetOwnerInfo(ownerId), result => Ok(result));
    }

    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> UpdateOwnerProfile([FromBody] DTO_UpdateUserInfoRequest request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(async () =>
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
    public Task<IActionResult> ChangeOwnerPassword([FromBody] ChangePasswordRequest request)
    {
        if (request == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(() => _owner.ChangeOwnerPassword(ownerId, request), _ => NoContent());
    }

    [HttpGet("offers/{centerId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetCenterOffers(int centerId)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(async () =>
        {
            var center = await _center.GetByID(centerId);
            if (center == null) throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerId) throw new UnauthorizedAccessException();
            return await _offer.GetByCenterId(centerId);
        }, result => Ok(result));
    }

    [HttpPost("offers")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<IActionResult> CreateOffer([FromBody] Offer offer)
    {
        if (offer == null) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(async () =>
        {
            var center = await _center.GetByID(offer.CenterId);
            if (center == null) throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerId) throw new UnauthorizedAccessException();
            if (!await _offer.Add(offer)) throw new Exception("Failed to create offer.");
            return offer;
        }, result => CreatedAtAction(nameof(GetCenterOffers), new { centerId = result.CenterId }, result));
    }

    [HttpPut("offers/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateOffer(int id, [FromBody] Offer offer)
    {
        if (offer == null || id != offer.OfferId) return Task.FromResult<IActionResult>(BadRequest());
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(async () =>
        {
            var existing = await _offer.GetByID(id) ?? throw new KeyNotFoundException("Offer not found.");
            var center = await _center.GetByID(existing.CenterId) ?? throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerId) throw new UnauthorizedAccessException();
            if (!await _offer.Update(offer)) throw new Exception("Failed to update offer.");
            return true;
        }, _ => NoContent());
    }

    [HttpDelete("offers/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteOffer(int id)
    {
        if (!TryGetOwnerId(out var ownerId)) return Task.FromResult<IActionResult>(Unauthorized());
        return Run(async () =>
        {
            var existing = await _offer.GetByID(id) ?? throw new KeyNotFoundException("Offer not found.");
            var center = await _center.GetByID(existing.CenterId) ?? throw new KeyNotFoundException("Center not found.");
            if (center.OwnerUserId != ownerId) throw new UnauthorizedAccessException();
            if (!await _offer.Delete(id)) throw new Exception("Failed to delete offer.");
            return true;
        }, _ => NoContent());
    }

    private bool TryGetOwnerId(out int ownerId)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out ownerId) && ownerId > 0;
    }

    private async Task<IActionResult> Run<T>(Func<Task<T>> action, Func<T, IActionResult> success)
    {
        try
        {
            return success(await action());
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Owner API operation failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }
}
