using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Player;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class TournamentPlayerDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(TournamentPlayer tp)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.TournamentPlayers.Add(tp);
                await db.SaveChangesAsync();
                return tp.Id;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add TournamentPlayer Error", ex);
                return 0;
            }
        }
        public static async Task<bool> DeleteByTournamentAndUser(int tournamentId, int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.TournamentPlayers.FirstOrDefaultAsync(tp => tp.TournamentId == tournamentId && tp.UserId == userId);
                if (existing == null) return false;
                db.TournamentPlayers.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete TournamentPlayer Error", ex);
                return false;
            }
        }
        public static async Task<List<DTO_PlayerTournamentInfo>?> GetByPlayerId(int playerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.TournamentPlayers
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
        public static async Task<List<TournamentPlayer>> GetByTournamentId(int tournamentId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.TournamentPlayers.Where(x => x.TournamentId == tournamentId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get TournamentPlayers By Tournament Error", ex);
                return new List<TournamentPlayer>();
            }
        }
        public static async Task<TournamentPlayer?> GetByTournamentAndUser(int tournamentId, int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.TournamentPlayers.AsNoTracking().FirstOrDefaultAsync(tp => tp.TournamentId == tournamentId && tp.UserId == userId);
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
