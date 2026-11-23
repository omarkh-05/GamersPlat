using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.DTOs.Pagination;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("top-10")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTopUnReadForUserBell([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = PaginationParameters.MaximumPageSize)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            return Ok(await _notificationService.GetTopUnReadForUserBell(userId));
        }
        catch (Exception)
        {
            return NotificationOperationFailed();
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNotifications([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = PaginationParameters.MaximumPageSize)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        try
        {
            return Ok(await _notificationService.GetForUserAsync(userId, pagination));
        }
        catch (Exception)
        {
            return NotificationOperationFailed();
        }
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUnreadCount()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            return Ok(new { count = await _notificationService.GetUnreadCountAsync(userId) });
        }
        catch (Exception)
        {
            return NotificationOperationFailed();
        }
    }

    [HttpPatch("{id:int}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkRead(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            return await _notificationService.MarkAsReadAsync(userId, id)
                ? NoContent()
                : NotFound();
        }
        catch (Exception)
        {
            return NotificationOperationFailed();
        }
    }

    [HttpPatch("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAllRead()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            await _notificationService.MarkAllAsReadAsync(userId);
            return NoContent();
        }
        catch (Exception)
        {
            return NotificationOperationFailed();
        }
    }

    private ObjectResult NotificationOperationFailed()
        => StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to process the notification request." });

    private bool TryGetUserId(out int userId)
        => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
}
