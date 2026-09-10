using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class GameDLL
    {
        public static int Add(Game game)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Games.Add(game);
                db.SaveChanges();
                return game.GameId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Game Error", ex);
                return 0;
            }
        }

        public static bool Update(Game game)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Games.FirstOrDefault(g => g.GameId == game.GameId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(game);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Game Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Games.FirstOrDefault(g => g.GameId == id);
                if (existing == null) return false;
                db.Games.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Game Error", ex);
                return false;
            }
        }

        public static async Task<Game?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Games
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.GameId == id);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Game By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<Game>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Games
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Games Error", ex);
                return new List<Game>();
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
