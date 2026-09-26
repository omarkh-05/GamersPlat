using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class GameDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(Game game)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Games.Add(game);
                await db.SaveChangesAsync();
                return game.GameId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Game Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Game game)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Games.FirstOrDefaultAsync(g => g.GameId == game.GameId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(game);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Game Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Games.FirstOrDefaultAsync(g => g.GameId == id);
                if (existing == null) return false;
                db.Games.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Game Error", ex);
                return false;
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
        // ================ CRUD ================


        // ================ Read By ================
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
        // ================ Read By ================
    }
}
