using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class TournamentDLL
    {
        private readonly GamersPlatDbContext _db;

        public TournamentDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ CRUD ================
        public async Task<int> Add(Tournament t)
        {
            try
            {
                _db.Tournaments.Add(t);
                await _db.SaveChangesAsync();
                return t.TournamentId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Tournament Error", ex);
                return 0;
            }
        }
        public async Task<bool> Update(Tournament t)
        {
            try
            {
                var existing = await _db.Tournaments.FirstOrDefaultAsync(x => x.TournamentId == t.TournamentId);
                if (existing == null) return false;
                _db.Entry(existing).CurrentValues.SetValues(t);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Tournament Error", ex);
                return false;
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.Tournaments.FirstOrDefaultAsync(x => x.TournamentId == id);
                if (existing == null) return false;
                _db.Tournaments.Remove(existing);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Tournament Error", ex);
                return false;
            }
        }
        public async Task<List<Tournament>> GetAll()
        {
            try
            {
                return await _db.Tournaments
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
        public async Task<Tournament?> GetByID(int id)
        {
            try
            {
                return await _db.Tournaments
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
