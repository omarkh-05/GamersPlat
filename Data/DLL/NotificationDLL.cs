using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class NotificationDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(Notification n)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Notifications.Add(n);
                await db.SaveChangesAsync();
                return n.NotificationId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Notification Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Notification n)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Notifications.FirstOrDefaultAsync(x => x.NotificationId == n.NotificationId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(n);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Notification Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Notifications.FirstOrDefaultAsync(x => x.NotificationId == id);
                if (existing == null) return false;
                db.Notifications.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Notification Error", ex);
                return false;
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
                EventLog_Helper.WriteEventLog("Get All Notifications Error", ex);
                return new List<Notification>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<Notification?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.NotificationId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Notification By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
