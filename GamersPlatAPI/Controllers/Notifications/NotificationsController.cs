using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notification;

        public NotificationsController(INotificationService notification)
        {
            _notification = notification;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var userId)) return Unauthorized();

            var all = await _notification.GetAll();
            var mine = all.Where(n => n.UserId == userId).ToList();
            return Ok(mine);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/notifications")]
        public async Task<IActionResult> Send([FromBody] Data.Notification n)
        {
            if (n == null) return BadRequest();
            if (!await _notification.Add(n)) return StatusCode(500);
            return Ok(n);
        }

        [Authorize]
        [HttpPut("read/{id:int}")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var existing = await _notification.GetByID(id);
            if (existing == null) return NotFound();
            existing.ReadAt = DateTime.UtcNow;
            if (!await _notification.Update(existing)) return StatusCode(500);
            return NoContent();
        }
    }
}
