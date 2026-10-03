using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class SessionParticipantDLL
    {
        private readonly GamersPlatDbContext _db;

        public SessionParticipantDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ CRUD ===========
        public async Task<int> Add(SessionParticipant sp)
        {
            try
            {
                _db.SessionParticipants.Add(sp);
                await _db.SaveChangesAsync();
                var id = sp.Id;
                return id;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add SessionParticipant Error", ex);
                return 0;
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = _db.SessionParticipants.SingleOrDefault(x => x.Id == id);
                if (existing == null) return false;
                _db.SessionParticipants.Remove(existing);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete SessionParticipant Error", ex);
                return false;
            }
        }
        // ================ CRUD ===========


        // ================ Read By ===========
        public async Task<SessionParticipant?> GetBySessionId(int sessionId)
        {
            try
            {
                return await _db.SessionParticipants.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId);
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
