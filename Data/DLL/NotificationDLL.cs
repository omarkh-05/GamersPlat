using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class NotificationDLL
    {
        public static int Add(Notification n)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Notifications.Add(n);
                db.SaveChanges();
                return n.NotificationId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Notification Error", ex);
                return 0;
            }
        }

        public static bool Update(Notification n)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Notifications.FirstOrDefault(x => x.NotificationId == n.NotificationId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(n);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Notification Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Notifications.FirstOrDefault(x => x.NotificationId == id);
                if (existing == null) return false;
                db.Notifications.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Notification Error", ex);
                return false;
            }
        }

        public static async Task<Notification?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.NotificationId == id);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Notification By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<Notification>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Notifications.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Notifications Error", ex);
                return new List<Notification>();
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
