using Bussiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var t = await TournamentBLL.GetAll();
            return Ok(t);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await TournamentBLL.GetByID(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpGet("results/{id:int}")]
        public async Task<IActionResult> GetResults(int id)
        {
            var t = await TournamentBLL.GetByID(id);
            if (t == null) return NotFound();

            // For now, tournament results are the list of players who joined, ordered by JoinedAt
            var participants = await TournamentPlayerBLL.GetByTournamentId(id);
            var users = participants.Select(p => new { p.UserId, p.JoinedAt }).ToList();

            var resp = new
            {
                TournamentId = id,
                TournamentName = t.TournamentName,
                Participants = users,
                Count = users.Count
            };

            return Ok(resp);
        }

        [Authorize]
        [HttpPost("join/{id:int}")]
        public async Task<IActionResult> Join(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var tournament = await TournamentBLL.GetByID(id);
            if (tournament == null) return NotFound("Tournament not found");

            // check if already joined
            var existing = await TournamentPlayerBLL.GetByTournamentAndUser(id, userId);
            if (existing != null) return Conflict("User already joined this tournament");

            // check capacity
            var participants = await TournamentPlayerBLL.GetByTournamentId(id);
            if (tournament.MaxPlayers.HasValue && participants.Count >= tournament.MaxPlayers.Value)
                return Conflict("Tournament is full");

            var tp = new Data.TournamentPlayer
            {
                TournamentId = id,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            var bll = new TournamentPlayerBLL(tp);
            if (!bll.Add()) return StatusCode(500, "Unable to join tournament");

            // return info
            var resp = new GamersPlatAPI.DTOs.TournamentJoinResponse
            {
                TournamentId = id,
                Message = "Joined",
                ParticipantsCount = participants.Count + 1
            };

            return Ok(resp);
        }

        [Authorize]
        [HttpDelete("leave/{id:int}")]
        public async Task<IActionResult> Leave(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var existing = await TournamentPlayerBLL.GetByTournamentAndUser(id, userId);
            if (existing == null) return NotFound();

            var ok = TournamentPlayerBLL.DeleteByTournamentAndUser(id, userId);
            if (!ok) return StatusCode(500, "Unable to leave tournament");

            return Ok(new { message = "Left tournament" });
        }
    }
}
