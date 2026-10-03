using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class ServiceDLL
    {
        private readonly GamersPlatDbContext _db;

        public ServiceDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ===============
        public async Task<int> Add(Service service)
        {
            try
            {
                _db.Services.Add(service);

                await _db.SaveChangesAsync();

                return service.ServiceId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Add Service Error", ex);

                return 0;
            }
        }

        public async Task<bool> Update(Service service)
        {
            try
            {
                var existing = await _db.Services
                    .FirstOrDefaultAsync(s =>
                        s.ServiceId == service.ServiceId &&
                        s.CenterId == service.CenterId);

                if (existing == null)
                    return false;

                existing.Name = service.Name;
                existing.IsActive = service.IsActive;
                existing.UpdatedAt = DateTime.UtcNow;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Update Service Error", ex);

                return false;
            }
        }

        public async Task<bool> Delete(int serviceId, int centerId)
        {
            try
            {
                var existing = await _db.Services
                    .FirstOrDefaultAsync(s =>
                        s.ServiceId == serviceId &&
                        s.CenterId == centerId);

                if (existing == null)
                    return false;

                _db.Services.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Delete Service Error", ex);

                return false;
            }
        }

        public async Task<bool> UpdateActiveStatus(int serviceId, int centerId)
        {
            try
            {
                var service = await _db.Services
                    .FirstOrDefaultAsync(s =>
                        s.ServiceId == serviceId &&
                        s.CenterId == centerId);

                if (service == null)
                    return false;

                service.IsActive = !service.IsActive;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Update Service Active Status Error", ex);

                return false;
            }
        }

        public async Task<List<Service>> GetAll()
        {
            try
            {
                return await _db.Services
                    .Include(s => s.Center)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get All Services Error", ex);

                return new List<Service>();
            }
        }

        // ================ CRUD ===============


        // ================ Read By ===============
        public async Task<Service?> GetByID(int serviceId)
        {
            try
            {
                return await _db.Services
                    .Include(s => s.Center)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Service By ID Error", ex);

                return null;
            }
        }

        // ================ Read By ===============
    }
}