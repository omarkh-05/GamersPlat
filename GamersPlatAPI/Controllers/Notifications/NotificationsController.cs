using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return Ok(await _notificationService.GetForUserAsync(userId));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return Ok(new { count = await _notificationService.GetUnreadCountAsync(userId) });
    }

    [HttpPatch("{id:int}/read")]
    [HttpPut("read/{id:int}")]
    public async Task<IActionResult> MarkRead(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return await _notificationService.MarkAsReadAsync(userId, id)
            ? NoContent()
            : NotFound();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }

    private bool TryGetUserId(out int userId)
        => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
}
