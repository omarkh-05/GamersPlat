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
        public async Task<IActionResult> GetAll()
        {
            var centers = await _center.GetAll();
            return Ok(centers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _center.GetByID(id);
            if (c == null) return NotFound();
            return Ok(c);
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
