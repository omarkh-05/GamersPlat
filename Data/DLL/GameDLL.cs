using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
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
                EventLog_Helper.WriteEventLog("Add Game Error", ex);
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
                EventLog_Helper.WriteEventLog("Update Game Error", ex);
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
                EventLog_Helper.WriteEventLog("Delete Game Error", ex);
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
                EventLog_Helper.WriteEventLog("Get Game By ID Error", ex);
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
                EventLog_Helper.WriteEventLog("Get All Games Error", ex);
                return new List<Game>();
            }
        }


    }
}
