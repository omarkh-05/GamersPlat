using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Centers
{
    [ApiController]
    [Route("api/centers/{centerId:int}/[controller]")]
    public class CenterImagesController : ControllerBase
    {
        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> Upload(int centerId, [FromForm] IFormFile file, [FromForm] bool isMain = false)
        {
            if (file == null || file.Length == 0) return BadRequest();

            // Ownership check
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await CenterBLL.GetByID(centerId);
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

            var bll = new CenterImageBLL(img);
            if (!bll.Add()) return StatusCode(500);

            return CreatedAtAction(nameof(GetById), new { centerId = centerId, id = bll._imageID }, img);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int centerId, int id)
        {
            var img = await CenterImageBLL.GetByID(id);
            if (img == null || img.CenterId != centerId) return NotFound();
            return Ok(img);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int centerId, int id)
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var img = await CenterImageBLL.GetByID(id);
            if (img == null) return NotFound();
            var center = await CenterBLL.GetByID(centerId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            var bll = new CenterImageBLL();
            if (!bll.Delete(id)) return StatusCode(500);

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
