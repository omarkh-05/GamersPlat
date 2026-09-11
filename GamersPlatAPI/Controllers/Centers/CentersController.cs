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
            var centers = await CenterBLL.GetAll();
            return Ok(centers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await CenterBLL.GetByID(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] Data.Center center)
        {
            if (center == null) return BadRequest();
            var bll = new CenterBLL(center);
            if (!bll.Add()) return StatusCode(500);
            return CreatedAtAction(nameof(GetById), new { id = bll._centerID }, center);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Data.Center center)
        {
            if (center == null || id != center.CenterId) return BadRequest();
            var bll = new CenterBLL(center);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var bll = new CenterBLL();
            if (!bll.Delete(id)) return NotFound();
            return NoContent();
        }
    }
}
