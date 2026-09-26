using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Owner
{
    [ApiController]
    [Route("api/owner/[controller]")]
    [Authorize(Roles = "Owner")]
    public class OwnerTournamentsController : ControllerBase
    {
        private readonly ITournamentService _tournaments;
        private readonly ICenterService _center;

        public OwnerTournamentsController(ITournamentService tournaments, ICenterService center)
        {
            _tournaments = tournaments;
            _center = center;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Data.Tournament t)
        {
            if (t == null) return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await _center.GetByID(t.CenterId);
            if (center == null) return NotFound();
            if (center.OwnerUserId != uid) return Forbid();
            if (!await _tournaments.Add(t)) return StatusCode(500);
            return CreatedAtAction("GetById", "Tournaments", new { id = _tournaments.LastId }, t);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Data.Tournament t)
        {
            if (t == null || t.TournamentId != id) return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var existing = await _tournaments.GetByID(id);
            if (existing == null) return NotFound();

            var center = await _center.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();
            if (!await _tournaments.Update(t)) return StatusCode(500);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _tournaments.GetByID(id);
            if (existing == null) return NotFound();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await _center.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();
            if (!await _tournaments.Delete(id)) return StatusCode(500);
            return NoContent();
        }
    }
}
