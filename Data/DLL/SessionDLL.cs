using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class SessionDLL
    {
        // ================ CRUD ===========
        public static async Task<int> Add(Session session)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Sessions.Add(session);
                await db.SaveChangesAsync();
                return session.SessionId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Session Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Session session)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Sessions.FirstOrDefaultAsync(s => s.SessionId == session.SessionId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(session);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Session Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int sessionId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Sessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
                if (existing == null) return false;
                db.Sessions.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Session Error", ex);
                return false;
            }
        }
        public static async Task<List<Session>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Sessions
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
        public static async Task<Session?> GetByID(int sessionId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Sessions
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
