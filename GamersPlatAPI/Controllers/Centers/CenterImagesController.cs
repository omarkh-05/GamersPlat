using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Centers
{
    [ApiController]
    [Route("api/centers/{centerId:int}/[controller]")]
    public class CenterImagesController : ControllerBase
    {
        private readonly ICenterService _center;
        private readonly ICenterImageService _images;

        public CenterImagesController(ICenterService center, ICenterImageService images)
        {
            _center = center;
            _images = images;
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(int centerId, IFormFile file, [FromForm] bool isMain = false)
        {
            if (file == null || file.Length == 0) return BadRequest();

            // Ownership check
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await _center.GetByID(centerId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();

            // Save file to wwwroot/uploads/centers/{centerId}/ and record URL
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "centers", centerId.ToString());
            Directory.CreateDirectory(uploads);
            var fileName = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/centers/{centerId}/{fileName}";

            var img = new Data.CenterImage
            {
                CenterId = centerId,
                ImageUrl = url,
                IsMain = isMain,
                CreatedAt = DateTime.UtcNow
            };

            if (!await _images.Add(img)) return StatusCode(500);

            return CreatedAtAction(nameof(GetById), new { centerId = centerId, id = _images.LastId }, img);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int centerId, int id)
        {
            var img = await _images.GetByID(id);
            if (img == null || img.CenterId != centerId) return NotFound();
            return Ok(img);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int centerId, int id)
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var img = await _images.GetByID(id);
            if (img == null) return NotFound();
            var center = await _center.GetByID(centerId);
            if (center == null || center.OwnerUserId != uid) return Forbid();
            if (!await _images.Delete(id)) return StatusCode(500);

            // attempt to delete file from disk (best-effort)
            try
            {
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(physicalPath)) System.IO.File.Delete(physicalPath);
            }
            catch { }

            return NoContent();
        }
    }
}
