using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Owner
{
    [ApiController]
    [Route("api/owner/[controller]")]
    [Authorize(Roles = "Owner")]
    public class OwnerTournamentsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Data.Tournament t)
        {
            if (t == null) return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await CenterBLL.GetByID(t.CenterId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();

            var bll = new TournamentBLL(t);
            if (!bll.Add()) return StatusCode(500);
            return CreatedAtAction("GetById", "Tournaments", new { id = bll._tID }, t);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Data.Tournament t)
        {
            if (t == null || t.TournamentId != id) return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var existing = await TournamentBLL.GetByID(id);
            if (existing == null) return NotFound();

            var center = await CenterBLL.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            var bll = new TournamentBLL(t);
            if (!bll.Update()) return StatusCode(500);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await TournamentBLL.GetByID(id);
            if (existing == null) return NotFound();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await CenterBLL.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();

            var bll = new TournamentBLL();
            if (!bll.Delete(id)) return StatusCode(500);
            return NoContent();
        }
    }
}
