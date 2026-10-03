using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class GameDLL
    {
        private readonly GamersPlatDbContext _db;

        public GameDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(Game game)
        {
            try
            {
                _db.Games.Add(game);
                await _db.SaveChangesAsync();

                return game.GameId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Add Game Error",
                    ex);

                return 0;
            }
        }

        public async Task<bool> Update(Game game)
        {
            try
            {
                var existing = await _db.Games
                    .FirstOrDefaultAsync(g => g.GameId == game.GameId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(game);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Update Game Error",
                    ex);

                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.Games
                    .FirstOrDefaultAsync(g => g.GameId == id);

                if (existing == null)
                    return false;

                _db.Games.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Delete Game Error",
                    ex);

                return false;
            }
        }

        public async Task<List<Game>> GetAll()
        {
            try
            {
                return await _db.Games
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get All Games Error",
                    ex);

                return new List<Game>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Game?> GetByID(int id)
        {
            try
            {
                return await _db.Games
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.GameId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Game By ID Error",
                    ex);

                return null;
            }
        }
        // ================ Read By ================
    }
}