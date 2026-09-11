using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(id, out var userId)) return Unauthorized();

            var all = await NotificationBLL.GetAll();
            var mine = all.Where(n => n.UserId == userId).ToList();
            return Ok(mine);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/notifications")]
        public IActionResult Send([FromBody] Data.Notification n)
        {
            if (n == null) return BadRequest();
            var bll = new NotificationBLL(n);
            if (!bll.Add()) return StatusCode(500);
            return Ok(n);
        }

        [Authorize]
        [HttpPut("read/{id:int}")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var existing = await NotificationBLL.GetByID(id);
            if (existing == null) return NotFound();
            existing.ReadAt = DateTime.UtcNow;
            var bll = new NotificationBLL(existing);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }
    }
}
