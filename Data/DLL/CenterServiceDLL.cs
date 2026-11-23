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
        public async Task<int> Add(CenterService service, int centerId)
        {
            try
            {
                var existingService = await _db.Services.FirstOrDefaultAsync(s => s.ServiceId == service.ServiceId);

                if (existingService == null)
                {
                    return 0;
                }

                var existingCenterService = await _db.CenterServices
                    .AnyAsync(cs => cs.CenterId == centerId && cs.ServiceId == existingService.ServiceId);

                if (existingCenterService)
                    return 0;

                _db.CenterServices.Add(service);

                await _db.SaveChangesAsync();

                return existingService.ServiceId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Service Error", ex);
                return 0;
            }
        }
        public async Task<bool> Update(CenterService service)
        {
            try
            {
                var existing = await _db.CenterServices
                    .FirstOrDefaultAsync(cs => cs.CenterServiceId == service.CenterServiceId);

                if (existing == null)
                    return false;

                existing.ServiceId = service.ServiceId;
                existing.IsActive = service.IsActive;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Service Error", ex);
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
                    .FirstOrDefaultAsync(cs => cs.ServiceId == (byte)serviceId && cs.CenterId == centerId);

                if (existing == null)
                    return false;

                _db.CenterServices.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Service Error", ex);

                return false;
            }
        }
        public async Task<bool> ActiveStatus(int centerServiceId, int centerId)
        {
            try
            {
                var service = await _db.CenterServices
                    .FirstOrDefaultAsync(cs => cs.CenterServiceId == centerServiceId);

                if (service == null)
                    return false;

                if (service.IsActive == true)
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
        public async Task<bool> DeactiveStatus(int centerServiceId, int centerId)
        {
            try
            {

                var service = await _db.CenterServices
                    .FirstOrDefaultAsync(cs => cs.CenterServiceId == centerServiceId);

                if (service == null)
                    return false;

                if (service.IsActive == false)
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
        public async Task<List<DTO_ServicesInfo>> GetAll()
        {
            try
            {
                return await _db.CenterServices
                    .AsNoTracking()
                    .Select(s => new DTO_ServicesInfo
                    {
                        CenterServiceId = s.CenterServiceId,
                        ServiceId = s.ServiceId,
                        Name = s.Service.Name,
                        Icon = s.Icon,
                        IsActive = s.IsActive
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Services Error", ex);

                return new List<DTO_ServicesInfo>();
            }
        }

        // ================ CRUD ===============


        // ================ Read By ===============
        public async Task<CenterService?> GetByID(int centerServiceId)
        {
            try
            {
                return await _db.CenterServices
                    .FirstOrDefaultAsync(s => s.CenterServiceId == centerServiceId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Service By ID Error", ex);

                return null;
            }
        }
        public async Task<List<DTO_ServicesInfo>?> GetAllByCenterId(int centerId)
        {
            try
            {
                return await _db.CenterServices
                    .AsNoTracking()
                    .Where(cs => cs.CenterId == centerId)
                    .Select(cs => new DTO_ServicesInfo
                    {
                        CenterServiceId = cs.CenterServiceId,
                        ServiceId = cs.ServiceId,
                        CenterId = cs.CenterId,
                        Name = cs.Service.Name,
                        Icon = cs.Icon,
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