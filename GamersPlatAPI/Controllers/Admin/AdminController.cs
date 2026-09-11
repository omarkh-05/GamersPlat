using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await UserBLL.GetAll();
            return Ok(users);
        }

        [HttpPut("block/{id:int}")]
        public IActionResult BlockUser(int id)
        {
            var task = UserBLL.GetByID(id);
            task.Wait();
            var user = task.Result;
            if (user == null) return NotFound();
            user.IsActive = false;
            var bll = new UserBLL(user);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [HttpPut("unblock/{id:int}")]
        public IActionResult UnblockUser(int id)
        {
            var task = UserBLL.GetByID(id);
            task.Wait();
            var user = task.Result;
            if (user == null) return NotFound();
            user.IsActive = true;
            var bll = new UserBLL(user);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [HttpGet("centers")]
        public async Task<IActionResult> GetCenters()
        {
            var centers = await CenterBLL.GetAll();
            return Ok(centers);
        }

        [HttpPut("approve/{id:int}")]
        public async Task<IActionResult> ApproveCenter(int id)
        {
            var center = await CenterBLL.GetByID(id);
            if (center == null) return NotFound();
            center.CenterStatus = "Approved";
            var bll = new CenterBLL(center);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [HttpPut("reject/{id:int}")]
        public async Task<IActionResult> RejectCenter(int id)
        {
            var center = await CenterBLL.GetByID(id);
            if (center == null) return NotFound();
            center.CenterStatus = "Rejected";
            var bll = new CenterBLL(center);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics()
        {
            var users = await UserBLL.GetAll();
            var centers = await CenterBLL.GetAll();
            var bookings = await BookingBLL.GetAll();
            return Ok(new { Users = users.Count, Centers = centers.Count, Bookings = bookings.Count });
        }
    }
}
