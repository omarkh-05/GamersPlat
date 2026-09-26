using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Owner
{
    [ApiController]
    [Route("api/owner/[controller]")]
    [Authorize(Roles = "Owner")]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourcesTypeService _resources;
        private readonly ICenterService _center;

        public ResourcesController(IResourcesTypeService resources, ICenterService center)
        {
            _resources = resources;
            _center = center;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Data.ResourcesType rt)
        {
            if (rt == null) return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            // ensure owner owns the center
            var center = await _center.GetByID(rt.CenterId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();
            if (!await _resources.Add(rt)) return StatusCode(500);
            return CreatedAtAction(nameof(GetById), new { id = _resources.LastId }, rt);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rt = await _resources.GetByID(id);
            if (rt == null) return NotFound();
            return Ok(rt);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Data.ResourcesType rt)
        {
            if (rt == null || rt.ResourcesTypeId != id) return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var existing = await _resources.GetByID(id);
            if (existing == null) return NotFound();

            var center = await _center.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();
            if (!await _resources.Update(rt)) return StatusCode(500);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _resources.GetByID(id);
            if (existing == null) return NotFound();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await _center.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();
            if (!await _resources.Delete(id)) return StatusCode(500);
            return NoContent();
        }
    }
}
