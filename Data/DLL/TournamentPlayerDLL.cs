using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class TournamentPlayerDLL
    {
        public static int Add(TournamentPlayer tp)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.TournamentPlayers.Add(tp);
                db.SaveChanges();
                return tp.Id;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add TournamentPlayer Error", ex);
                return 0;
            }
        }

        public static async Task<List<TournamentPlayer>> GetByTournamentId(int tournamentId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.TournamentPlayers.Where(x => x.TournamentId == tournamentId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get TournamentPlayers By Tournament Error", ex);
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
                WriteEventLog("Get TournamentPlayer By Tournament and User Error", ex);
                return null;
            }
        }

        public static bool DeleteByTournamentAndUser(int tournamentId, int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.TournamentPlayers.FirstOrDefault(tp => tp.TournamentId == tournamentId && tp.UserId == userId);
                if (existing == null) return false;
                db.TournamentPlayers.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete TournamentPlayer Error", ex);
                return false;
            }
        }

        private static void WriteEventLog(string title, Exception ex)
        {
            string error = ex.Message;
            if (ex.InnerException != null)
                error += "\nInner Exception: " + ex.InnerException.Message;
            EventLog.WriteEntry("Application", $"{title}: {error}", EventLogEntryType.Error);
        }
    }
}
