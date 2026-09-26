using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _review;

        public ReviewsController(IReviewService review)
        {
            _review = review;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Data.Review review)
        {
            if (review == null) return BadRequest();
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(id, out var uid)) review.UserId = uid;
            if (!await _review.Add(review)) return StatusCode(500);
            return CreatedAtAction(nameof(GetById), new { id = _review.LastId }, review);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _review.GetByID(id);
            if (r == null) return NotFound();
            return Ok(r);
        }
    }
}
