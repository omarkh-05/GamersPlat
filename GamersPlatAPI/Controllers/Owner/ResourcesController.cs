using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Owner
{
    [ApiController]
    [Route("api/owner/[controller]")]
    [Authorize(Roles = "Owner")]
    public class ResourcesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Data.ResourcesType rt)
        {
            if (rt == null) return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            // ensure owner owns the center
            var center = await CenterBLL.GetByID(rt.CenterId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();

            var bll = new ResourcesTypeBLL(rt);
            if (!bll.Add()) return StatusCode(500);
            return CreatedAtAction(nameof(GetById), new { id = bll._rtID }, rt);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rt = ResourcesTypeBLL.GetByID(id);
            if (rt == null) return NotFound();
            return Ok(rt);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Data.ResourcesType rt)
        {
            if (rt == null || rt.ResourcesTypeId != id) return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var existing = ResourcesTypeBLL.GetByID(id);
            if (existing == null) return NotFound();

            var center = await CenterBLL.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            var bll = new ResourcesTypeBLL(rt);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = ResourcesTypeBLL.GetByID(id);
            if (existing == null) return NotFound();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await CenterBLL.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            var bll = new ResourcesTypeBLL();
            if (!bll.Delete(id)) return StatusCode(500);
            return NoContent();
        }
    }
}
