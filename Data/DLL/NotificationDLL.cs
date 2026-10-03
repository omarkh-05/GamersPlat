using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class NotificationDLL
    {
        private readonly GamersPlatDbContext _db;

        public NotificationDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================

        public async Task<int> Add(Notification n)
        {
            try
            {
                _db.Notifications.Add(n);
                await _db.SaveChangesAsync();
                return n.NotificationId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Notification Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(Notification n)
        {
            try
            {
                var existing = await _db.Notifications
                    .FirstOrDefaultAsync(x => x.NotificationId == n.NotificationId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(n);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Notification Error", ex);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.Notifications
                    .FirstOrDefaultAsync(x => x.NotificationId == id);

                if (existing == null)
                    return false;

                _db.Notifications.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Notification Error", ex);
                return false;
            }
        }

        public async Task<List<Notification>> GetAll()
        {
            try
            {
                return await _db.Notifications
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Notifications Error", ex);
                return new List<Notification>();
            }
        }

        // ================ CRUD ================


        // ================ Read By ================

        public async Task<Notification?> GetByID(int id)
        {
            try
            {
                return await _db.Notifications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.NotificationId == id);
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