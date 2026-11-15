using Data.EF;
using DataLayer;
using Domain.DTOs.Service;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Data.DLL
{
    public class ServiceDLL
    {
        private readonly GamersPlatDbContext _db;

        public ServiceDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(string name)
        {
            try
            {
                var service = new Service
                {
                    Name = name,
                    CreatedAt = DateTime.Now
                };
                _db.Services.Add(service);
                await _db.SaveChangesAsync();
                return service.ServiceId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Service Error",ex);

                return 0;
            }
        }
        public async Task<bool> Update(string name,byte serviceId)
        {
            try
            {
                var service = await _db.Services
                    .Where(x => x.ServiceId == serviceId)
                    .FirstOrDefaultAsync();
                if (service == null)
                    return false;
                var serviceDTO = new Service
                {
                    Name = name,
                    UpdatedAt = DateTime.Now
                };
                _db.Entry(service).CurrentValues.SetValues(serviceDTO);
                return _db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Service Error", ex);

                return false;
            }
        }
        public async Task<bool> Delete(byte serviceId)
        {
            try
            {
                var service = await _db.Services
                    .Where(x => x.ServiceId == serviceId)
                    .FirstOrDefaultAsync();
                if (service != null)
                    _db.Services.Remove(service);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Service Error", ex);
                return false;
            }
        }
        public async Task<List<DTO_Services>> GetAll()
        {
            try
            {
                return await _db.Services
                    .AsNoTracking()
                    .Select(x => new DTO_Services
                    {
                        ServiceId = x.ServiceId,
                        Name = x.Name
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Services Error", ex);
                return new List<DTO_Services>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Service?> GetById(byte serviceId)
        {
            try
            {
                return await _db.Services
                    .Where(x => x.ServiceId == serviceId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Service By Id Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
