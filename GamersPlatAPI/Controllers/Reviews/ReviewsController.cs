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
        public async Task<IActionResult> Create([FromBody] Domain.DTOs.CreateReviewDTO request)
        {
            if (request == null)
                return BadRequest();

            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(id, out var uid))
                return Unauthorized();

            var review = new Data.Review
            {
                UserId = uid,
                CenterId = request.CenterId,
                BookingId = request.BookingId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            if (!await _review.Add(review))
                return StatusCode(500);

            return CreatedAtAction(
                nameof(GetById),
                new { id = _review.LastId },
                new
                {
                    ReviewId = _review.LastId,
                    UserId = review.UserId,
                    CenterId = review.CenterId,
                    BookingId = review.BookingId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                });

        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _review.GetByID(id);

            if (r == null)
                return NotFound();

            return Ok(new
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                CenterId = r.CenterId,
                BookingId = r.BookingId,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                UserName = r.User?.FullName,
                CenterName = r.Center?.CenterName
            });
        }


    }
}
