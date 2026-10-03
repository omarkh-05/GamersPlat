using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class DeviceDLL
    {
        private readonly GamersPlatDbContext _db;

        public DeviceDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(Device device)
        {
            try
            {
                _db.Devices.Add(device);
                await _db.SaveChangesAsync();
                return device.DeviceId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Device Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(Device device)
        {
            try
            {
                var existing = await _db.Devices
                    .FirstOrDefaultAsync(d => d.DeviceId == device.DeviceId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(device);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Device Error", ex);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.Devices
                    .FirstOrDefaultAsync(d => d.DeviceId == id);

                if (existing == null)
                    return false;

                _db.Devices.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Device Error", ex);
                return false;
            }
        }

        public async Task<List<Device>> GetAll()
        {
            try
            {
                return await _db.Devices
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Devices Error", ex);
                return new List<Device>();
            }
        }

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Device?> GetByID(int id)
        {
            try
            {
                return await _db.Devices
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DeviceId == id);
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