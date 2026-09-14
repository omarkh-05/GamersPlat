using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class TournamentDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(Tournament t)
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
                EventLog_Helper.WriteEventLog("Add Tournament Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Tournament t)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Tournaments.FirstOrDefaultAsync(x => x.TournamentId == t.TournamentId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(t);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Tournament Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Tournaments.FirstOrDefaultAsync(x => x.TournamentId == id);
                if (existing == null) return false;
                db.Tournaments.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Tournament Error", ex);
                return false;
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
                EventLog_Helper.WriteEventLog("Get All Tournaments Error", ex);
                return new List<Tournament>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
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
                EventLog_Helper.WriteEventLog("Get Tournament By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
