using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class CenterInvitationDLL
    {
        public static int Add(CenterInvitation inv)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.CenterInvitations.Add(inv);
                db.SaveChanges();
                return inv.InvitationId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add CenterInvitation Error", ex);
                return 0;
            }
        }

        public static bool Update(CenterInvitation inv)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.CenterInvitations.FirstOrDefault(i => i.InvitationId == inv.InvitationId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(inv);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update CenterInvitation Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.CenterInvitations.FirstOrDefault(i => i.InvitationId == id);
                if (existing == null) return false;
                db.CenterInvitations.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete CenterInvitation Error", ex);
                return false;
            }
        }

        public static async Task<CenterInvitation?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.CenterInvitations.AsNoTracking().FirstOrDefaultAsync(i => i.InvitationId == id);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get CenterInvitation By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<CenterInvitation>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.CenterInvitations.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All CenterInvitations Error", ex);
                return new List<CenterInvitation>();
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
