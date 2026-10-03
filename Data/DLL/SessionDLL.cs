using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class SessionDLL
    {
        private readonly GamersPlatDbContext _db;

        public SessionDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ CRUD ===========
        public async Task<int> Add(Session session)
        {
            try
            {
                _db.Sessions.Add(session);
                await _db.SaveChangesAsync();
                return session.SessionId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Session Error", ex);
                return 0;
            }
        }
        public async Task<bool> Update(Session session)
        {
            try
            {
                var existing = await _db.Sessions.FirstOrDefaultAsync(s => s.SessionId == session.SessionId);
                if (existing == null) return false;
                _db.Entry(existing).CurrentValues.SetValues(session);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Session Error", ex);
                return false;
            }
        }
        public async Task<bool> Delete(int sessionId)
        {
            try
            {
                var existing = await _db.Sessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
                if (existing == null) return false;
                _db.Sessions.Remove(existing);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Session Error", ex);
                return false;
            }
        }
        public async Task<List<Session>> GetAll()
        {
            try
            {
                return await _db.Sessions
                    .Include(s => s.Center)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Sessions Error", ex);
                return new List<Session>();
            }
        }
        // ================ CRUD ===========

        
        // ================ Read By ===========
        public async Task<Session?> GetByID(int sessionId)
        {
            try
            {
                return await _db.Sessions
                    .Include(s => s.CreatedByUser)
                    .Include(s => s.Device)
                    .Include(s => s.Game)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Session By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ===========
    }
}
