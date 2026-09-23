using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public AdminController(IUser user, CenterBLL center, BookingBLL booking)
        {
            _user = user;
            _center = center;
            _booking = booking;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _user.GetAll();
            return Ok(users);
        }

        [HttpPut("block/{id:int}")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var user = await _user.GetById(id);
            if (user == null) return NotFound();
            user.IsActive = false;
            if (! await _user.Update(id, user)) return StatusCode(500);
            return NoContent();
        }

        [HttpPut("unblock/{id:int}")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var user = await _user.GetById(id);
            if (user == null) return NotFound();
            user.IsActive = true;
            if (! await _user.Update(id, user)) return StatusCode(500);
            return NoContent();
        }

        [HttpGet("centers")]
        public async Task<IActionResult> GetCenters()
        {
            var centers = await _center.GetAll();
            return Ok(centers);
        }

        //[HttpPut("approve/{id:int}")]
        //public async Task<IActionResult> ApproveCenter(int id)
        //{
        //    var center = await _center.GetByID(id);
        //    if (center == null) return NotFound();
        //    center.CenterStatus = "Approved";
        //    if (! await _center.Update(center, id)) return StatusCode(500);
        //    return NoContent();
        //}

        //[HttpPut("reject/{id:int}")]
        //public async Task<IActionResult> RejectCenter(int id)
        //{
        //    var center = await _center.GetByID(id);
        //    if (center == null) return NotFound();
        //    center.CenterStatus = "Rejected";
        //    if (! await _center.Update(center, id)) return StatusCode(500);
        //    return NoContent();
        //}

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics()
        {
            var users = await _user.GetAll();
            var centers = await _center.GetAll();
            var bookings = await _booking.GetAll();
            return Ok(new { Users = users.Count, Centers = centers.Count, Bookings = bookings.Count });
        }
    }
}
