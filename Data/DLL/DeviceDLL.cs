using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class DeviceDLL
    {
        public static int Add(Device device)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Devices.Add(device);
                db.SaveChanges();
                return device.DeviceId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Device Error", ex);
                return 0;
            }
        }

        public static bool Update(Device device)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Devices.FirstOrDefault(d => d.DeviceId == device.DeviceId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(device);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Device Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Devices.FirstOrDefault(d => d.DeviceId == id);
                if (existing == null) return false;
                db.Devices.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Device Error", ex);
                return false;
            }
        }

        public static async Task<Device?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Devices.AsNoTracking().FirstOrDefaultAsync(d => d.DeviceId == id);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Device By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<Device>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Devices.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Devices Error", ex);
                return new List<Device>();
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
