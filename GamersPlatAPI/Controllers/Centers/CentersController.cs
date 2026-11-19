using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CentersController : ControllerBase
    {
        private readonly ICenterService _center;
        public CentersController(ICenterService center)
        {
            _center = center;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var excludedStatuses = new[] { "pending", "approved", "rejected", "inactive" };
            var centers = await _center.GetAll();
            var homeCenter = centers.Where(c => !excludedStatuses.Contains(c.CenterStatus));
            return Ok(homeCenter);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _center.GetCenterDetailsById(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpGet("slug/{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var center = await _center.GetCenterDetailsBySlug(slug);
            if (center == null) return NotFound();
            return Ok(center);
        }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] Data.Center center)
        //{
        //    if (center == null) return BadRequest();
        //    var bll = new CenterBLL();
        //    if (!await bll.Add(center)) return StatusCode(500);
        //    return CreatedAtAction(nameof(GetById), new { id = bll._centerID }, center);
        //}

        //[Authorize]
        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, [FromBody] Data.Center center)
        //{
        //    if (center == null || id != center.CenterId) return BadRequest();
        //    var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    int.TryParse(idClaim, out var uid);
        //    var bll = new CenterBLL();
        //    if (!await bll.Update(center, uid)) return StatusCode(500);
        //    return NoContent();
        //}

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _center.Delete(id)) return NotFound();
            return NoContent();
        }
    }
}
