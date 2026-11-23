using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Centers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "owner")]
    public class CenterImagesController : ControllerBase
    {
        private const long MaxUploadBytes = 8 * 1024 * 1024;

        private readonly ICenterService _center;
        private readonly ICenterImageService _images;

        public CenterImagesController(ICenterService center, ICenterImageService images)
        {
            _center = center;
            _images = images;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(8 * 1024 * 1024)]
        public async Task<IActionResult> Upload(int centerId, IFormFile file, [FromForm] bool isMain = false)
        {
            if (file == null || file.Length == 0 || file.Length > MaxUploadBytes) return BadRequest();

            // Ownership check
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await _center.GetByID(centerId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();

            using var input = file.OpenReadStream();
            var result = await _images.Upload(centerId, input, isMain);

            return result.Status switch
            {
                ImageUploadStatus.Success => CreatedAtAction(
                    nameof(GetById), new { centerId = centerId, id = _images.LastId }, result.Image),
                ImageUploadStatus.InvalidImage => BadRequest(),
                _ => StatusCode(500)
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int centerId)
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await _center.GetByID(centerId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();

            var images = await _images.GetAll();
            return Ok(images
                .Where(image => image.CenterId == centerId)
                .OrderBy(image => image.ImageId));
        }

        [HttpGet("{imageId:int}")]
        public async Task<IActionResult> GetById(int centerId, int imageId)
        {
            var img = await _images.GetByID(imageId);
            if (img == null || img.CenterId != centerId) return NotFound();
            return Ok(img);
        }

        [HttpDelete("{imageId:int}")]
        public async Task<IActionResult> Delete(int centerId, int imageId)
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var img = await _images.GetByID(imageId);
            if (img == null || img.CenterId != centerId) return NotFound();

            var center = await _center.GetByID(centerId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            if (!await _images.DeleteWithFiles(img)) return StatusCode(500);

            return NoContent();
        }
    }
}