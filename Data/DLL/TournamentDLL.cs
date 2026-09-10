using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class TournamentDLL
    {
        public static int Add(Tournament t)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Tournaments.Add(t);
                db.SaveChanges();
                return t.TournamentId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Tournament Error", ex);
                return 0;
            }
        }

        public static bool Update(Tournament t)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Tournaments.FirstOrDefault(x => x.TournamentId == t.TournamentId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(t);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Tournament Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Tournaments.FirstOrDefault(x => x.TournamentId == id);
                if (existing == null) return false;
                db.Tournaments.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Tournament Error", ex);
                return false;
            }
        }

        public static async Task<Tournament?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Tournaments
                    .Include(x => x.Center)
                    .Include(x => x.Game)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TournamentId == id);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Tournament By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<Tournament>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Tournaments
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Tournaments Error", ex);
                return new List<Tournament>();
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
