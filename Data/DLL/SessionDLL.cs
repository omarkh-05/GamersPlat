using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class SessionDLL
    {
        public static int Add(Session session)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Sessions.Add(session);
                db.SaveChanges();
                return session.SessionId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Session Error", ex);
                return 0;
            }
        }

        public static bool Update(Session session)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Sessions.FirstOrDefault(s => s.SessionId == session.SessionId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(session);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Session Error", ex);
                return false;
            }
        }

        public static bool Delete(int sessionId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Sessions.FirstOrDefault(s => s.SessionId == sessionId);
                if (existing == null) return false;
                db.Sessions.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Session Error", ex);
                return false;
            }
        }

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
                WriteEventLog("Get Session By ID Error", ex);
                return null;
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
                WriteEventLog("Get All Sessions Error", ex);
                return new List<Session>();
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
