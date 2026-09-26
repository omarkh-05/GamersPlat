using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class DeviceDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(Device device)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Devices.Add(device);
                await db.SaveChangesAsync();
                return device.DeviceId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Device Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Device device)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Devices.FirstOrDefaultAsync(d => d.DeviceId == device.DeviceId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(device);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Device Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
                if (existing == null) return false;
                db.Devices.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Device Error", ex);
                return false;
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
                EventLog_Helper.WriteEventLog("Get All Devices Error", ex);
                return new List<Device>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<Device?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Devices.AsNoTracking().FirstOrDefaultAsync(d => d.DeviceId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Device By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
