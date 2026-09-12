using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] Data.Review review)
        {
            if (review == null) return BadRequest();
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(id, out var uid)) review.UserId = uid;
            var bll = new ReviewBLL(review);
            if (!bll.Add()) return StatusCode(500);
            return CreatedAtAction(nameof(GetById), new { id = bll._reviewID }, review);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await ReviewBLL.GetByID(id);
            if (r == null) return NotFound();
            return Ok(r);
        }
    }
}
