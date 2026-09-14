using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class SessionParticipantDLL
    {
        // ================ CRUD ===========
        public static async Task<int> Add(SessionParticipant sp)
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
                EventLog_Helper.WriteEventLog("Add SessionParticipant Error", ex);
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
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete SessionParticipant Error", ex);
                return false;
            }
        }
        // ================ CRUD ===========


        // ================ Read By ===========
        public static async Task<SessionParticipant?> GetBySessionId(int sessionId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.SessionParticipants.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get SessionParticipant By Session Error", ex);
                return null;
            }
        }
        // ================ Read By ===========
    }
}
