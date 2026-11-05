using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Service;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class CenterServiceDLL
    {
        private readonly GamersPlatDbContext _db;

        public CenterServiceDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ===============
        public async Task<int> Add(Service service, int centerId)
        {
            try
            {
                var isActiveForCenter = service.IsActive;
                var existingService = await _db.Services
                    .FirstOrDefaultAsync(s => s.Name == service.Name);

                if (existingService == null)
                {
                    service.IsActive = true;
                    _db.Services.Add(service);
                }

                var existingCenterService = await _db.CenterServices
                    .AnyAsync(cs => cs.CenterId == centerId && cs.ServiceId == existingService.ServiceId);

                if (existingCenterService)
                    return 0;

                _db.CenterServices.Add(new CenterService
                {
                    CenterId = centerId,
                    Service = existingService,
                    IsActive = isActiveForCenter,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();

                return existingService.ServiceId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog( "Add Service Error", ex);

                return 0;
            }
        }

        public async Task<bool> Update(Service service)
        {
            try
            {
                var existing = await _db.CenterServices
                    .Include(cs => cs.Service)
                    .FirstOrDefaultAsync(cs => cs.ServiceId == service.ServiceId && cs.CenterId == service.ServiceId);

                if (existing == null)
                    return false;

                existing.Service.Name = service.Name;
                existing.Service.UpdatedAt = DateTime.UtcNow;
                existing.IsActive = service.IsActive;

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
                if (serviceId <= 0 || serviceId > byte.MaxValue)
                    return false;

                var existing = await _db.CenterServices
                    .FirstOrDefaultAsync(cs =>
                        cs.ServiceId == (byte)serviceId &&
                        cs.CenterId == centerId);

                if (existing == null)
                    return false;

                _db.CenterServices.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Delete Service Error", ex);

                return false;
            }
        }

        public async Task<bool> ActiveStatus(int serviceId, int centerId)
        {
            try
            {
                if (serviceId <= 0 || serviceId > byte.MaxValue)
                    return false;

                var service = await _db.CenterServices
                    .FirstOrDefaultAsync(cs => cs.ServiceId == (byte)serviceId && cs.CenterId == centerId);

                if (service == null)
                    return false;

                service.IsActive = true;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Active Service Active Status Error", ex);

                return false;
            }
        }
        public async Task<bool> DeactiveStatus(int serviceId, int centerId)
        {
            try
            {
                if (serviceId <= 0 || serviceId > byte.MaxValue)
                    return false;

                var service = await _db.CenterServices
                    .FirstOrDefaultAsync(cs => cs.ServiceId == (byte)serviceId && cs.CenterId == centerId);

                if (service == null)
                    return false;

                service.IsActive = false;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Active Service Active Status Error", ex);

                return false;
            }
        }

        public async Task<List<Service>> GetAll()
        {
            try
            {
                return await _db.Services
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
        public async Task<List<DTO_ServicesInfo>?> GetAllByCenterId(int centerId)
        {
            try
            {
                return await _db.CenterServices
                    .Include(cs => cs.Service)
                    .AsNoTracking()
                    .Where(cs => cs.CenterId == centerId)
                    .Select(cs => new DTO_ServicesInfo
                    {
                        ServiceId = cs.ServiceId,
                        CenterId = cs.CenterId,
                        Name = cs.Service.Name,
                        IsActive = cs.IsActive
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Service By ID Error", ex);

                return null;
            }
        }
        // ================ Read By ===============
    }
}