using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class SessionParticipantDLL
    {
        public static int Add(SessionParticipant sp)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.SessionParticipants.Add(sp);
                db.SaveChanges();
                var id = sp.Id;
                return id;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add SessionParticipant Error", ex);
                return 0;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.SessionParticipants.SingleOrDefault(x => x.Id == id);
                if (existing == null) return false;
                db.SessionParticipants.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete SessionParticipant Error", ex);
                return false;
            }
        }

        public static async Task<SessionParticipant?> GetBySessionId(int sessionId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.SessionParticipants.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get SessionParticipant By Session Error", ex);
                return null;
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
