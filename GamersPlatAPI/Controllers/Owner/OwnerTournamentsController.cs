using Bussiness;
using Bussiness.Interfaces;
using Domain.DTOs.Owner;
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
        public async Task<IActionResult> Create([FromBody] CreateTournamentDTO request)
        {
            if (request == null)
                return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid))
                return Unauthorized();

            var center = await _center.GetByID(request.CenterId);
            if (center == null)
                return NotFound();

            if (center.OwnerUserId != uid)
                return Forbid();

            var tournament = new Data.Tournament
            {
                CenterId = request.CenterId,
                TournamentName = request.TournamentName,
                GameId = request.GameId,
                DeviceId = request.DeviceId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MaxPlayers = request.MaxPlayers,
                WinnerUserId = request.WinnerUserId,
                RewardPoints = request.RewardPoints,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            if (!await _tournaments.Add(tournament))
                return StatusCode(500);

            return CreatedAtAction(
                "GetById",
                "Tournaments",
                new { id = _tournaments.LastId },
                new
                {
                    TournamentId = _tournaments.LastId,
                    tournament.CenterId,
                    tournament.TournamentName,
                    tournament.GameId,
                    tournament.DeviceId,
                    tournament.StartDate,
                    tournament.EndDate,
                    tournament.MaxPlayers,
                    tournament.WinnerUserId,
                    tournament.RewardPoints,
                    tournament.Description
                });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTournamentDTO request)
        {
            if (request == null)
                return BadRequest();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid))
                return Unauthorized();

            var existing = await _tournaments.GetByID(id);
            if (existing == null)
                return NotFound();

            var center = await _center.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid)
                return Forbid();

            var tournament = new Data.Tournament
            {
                TournamentId = existing.TournamentId,
                CenterId = existing.CenterId,
                TournamentName = request.TournamentName,
                GameId = request.GameId,
                DeviceId = request.DeviceId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MaxPlayers = request.MaxPlayers,
                WinnerUserId = request.WinnerUserId,
                RewardPoints = request.RewardPoints,
                Description = request.Description,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            if (!await _tournaments.Update(tournament))
                return StatusCode(500);

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
