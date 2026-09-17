using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CentersController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var centers = await new CenterBLL().GetAll();
            return Ok(centers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await new CenterBLL().GetByID(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Data.Center center)
        {
            if (center == null) return BadRequest();
            var bll = new CenterBLL();
            if (!await bll.Add(center)) return StatusCode(500);
            return CreatedAtAction(nameof(GetById), new { id = bll._centerID }, center);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Data.Center center)
        {
            if (center == null || id != center.CenterId) return BadRequest();
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(idClaim, out var uid);
            var bll = new CenterBLL();
            if (!await bll.Update(center, uid)) return StatusCode(500);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bll = new CenterBLL();
            if (!await bll.Delete(id)) return NotFound();
            return NoContent();
        }
    }
}
