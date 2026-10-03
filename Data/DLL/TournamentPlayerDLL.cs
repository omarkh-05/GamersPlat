using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Player;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class TournamentPlayerDLL
    {
        private readonly GamersPlatDbContext _db;

        public TournamentPlayerDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ CRUD ================
        public async Task<int> Add(TournamentPlayer tp)
        {
            try
            {
                _db.TournamentPlayers.Add(tp);
                await _db.SaveChangesAsync();
                return tp.Id;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add TournamentPlayer Error", ex);
                return 0;
            }
        }
        public async Task<bool> DeleteByTournamentAndUser(int tournamentId, int userId)
        {
            try
            {
                var existing = await _db.TournamentPlayers.FirstOrDefaultAsync(tp => tp.TournamentId == tournamentId && tp.UserId == userId);
                if (existing == null) return false;
                _db.TournamentPlayers.Remove(existing);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete TournamentPlayer Error", ex);
                return false;
            }
        }
        public async Task<List<DTO_PlayerTournamentInfo>?> GetByPlayerId(int playerId)
        {
            try
            {
                return await _db.TournamentPlayers
                 .Where(tp => tp.UserId == playerId)
                 .Select(tp => new DTO_PlayerTournamentInfo
                 {
                     CenterName = tp.Tournament.Center.CenterName,
                     TournamentName = tp.Tournament.TournamentName,
                     GameName = tp.Tournament.Game.GameName,
                     DeviceName = tp.Tournament.Device.DeviceName,
                     WinnerUserId = tp.Tournament.WinnerUser.FullName,
                     RewardPoints = tp.Tournament.RewardPoints,
                     JoinedAt = tp.JoinedAt
                 })
                 .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get TournamentPlayer By Tournament and User Error", ex);
                return null;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<TournamentPlayer>> GetByTournamentId(int tournamentId)
        {
            try
            {
                return await _db.TournamentPlayers.Where(x => x.TournamentId == tournamentId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get TournamentPlayers By Tournament Error", ex);
                return new List<TournamentPlayer>();
            }
        }
        public async Task<TournamentPlayer?> GetByTournamentAndUser(int tournamentId, int userId)
        {
            try
            {
                return await _db.TournamentPlayers.AsNoTracking().FirstOrDefaultAsync(tp => tp.TournamentId == tournamentId && tp.UserId == userId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get TournamentPlayer By Tournament and User Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
