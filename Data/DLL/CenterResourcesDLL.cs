using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Resource;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class CenterResourcesDLL
    {
        private readonly GamersPlatDbContext _db;

        public CenterResourcesDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ CRUD ===========
        public async Task<int> Add(CenterResource rt)
        {
            try
            {
                _db.CenterResources.Add(rt);
                await _db.SaveChangesAsync();
                return rt.ResourcesTypeId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Center Resource Error", ex);
                return 0;
            }
        }
        public async Task<bool> Update(CenterResource rt)
        {
            try
            { 
                var existing = await _db.CenterResources.FirstOrDefaultAsync(r => r.CenterResourceId == rt.CenterResourceId && r.CenterId == rt.CenterId);
                if (existing == null) return false;

                _db.Entry(existing).CurrentValues.SetValues(rt);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update ResourcesType Error", ex);
                return false;
            }
        }
        public async Task<bool> ActiveStatus(int centerId,int centerResourceId)
        {
            try
            {
                var resource = await _db.CenterResources.FirstOrDefaultAsync(r =>r.CenterResourceId == centerResourceId && r.CenterId == centerId);

                if (resource == null)
                    return false;

                if (resource.IsActive == true)
                    return false;
                
                resource.IsActive = true;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("ResourcesType Active Status Error", ex);
                return false;
            }
        }
        public async Task<bool> DeactiveStatus(int centerId, int centerResourceId)
        {
            try
            {
                var resource = await _db.CenterResources.FirstOrDefaultAsync(r => r.CenterResourceId == centerResourceId && r.CenterId == centerId);

                if (resource == null)
                    return false;

                if (resource.IsActive == false)
                    return false;

                resource.IsActive = false;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("ResourcesType Deactive Status Error", ex);
                return false;
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.CenterResources.FirstOrDefaultAsync(r => r.ResourcesTypeId == id);
                if (existing == null) return false;
                _db.CenterResources.Remove(existing);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Center Resource Error", ex);
                return false;
            }
        }
        public async Task<List<DTO_ResourceInfo>> GetAll()
        {
            try
            {
                return await _db.CenterResources.AsNoTracking().Select(r => new DTO_ResourceInfo
                {
                    CenterResourceId = r.CenterResourceId,
                    ResourcesTypeId = r.ResourcesTypeId,
                    CenterId = r.CenterId,
                    ResourceName = r.ResourcesType.ResourceName,
                    HourlyPrice = r.HourlyPrice,
                    TotalQuantity = r.TotalQuantity,
                    RoomType = r.RoomType,
                    Description = r.Description,
                    IsActive = r.IsActive
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Center Resources Error", ex);
                return new List<DTO_ResourceInfo>();
            }
        }
        // ================ CRUD ===========


        // ================ Read By ===========
        public async Task<CenterResource?> GetByID(int centerResourceId)
        {
            try
            {
                return await _db.CenterResources.FirstOrDefaultAsync(r => r.CenterResourceId == centerResourceId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get ResourcesType By ID Error", ex);
                return null;
            }
        }
        public async Task<List<DTO_ResourceInfo>> GetAllByCenterId(int centerId)
        {
            try
            {
                return await _db.CenterResources
                    .AsNoTracking()
                    .Where(r => r.CenterId == centerId)
                    .Select(r => new DTO_ResourceInfo
                    {
                        CenterResourceId = r.CenterResourceId,
                        ResourcesTypeId = r.ResourcesTypeId,
                        CenterId = r.CenterId,
                        ResourceName = r.ResourcesType.ResourceName,
                        HourlyPrice = r.HourlyPrice,
                        TotalQuantity = r.TotalQuantity,
                        RoomType = r.RoomType,
                        Description = r.Description,
                        IsActive = r.IsActive
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get ResourcesType By Center ID Error", ex);
                return new List<DTO_ResourceInfo>();
            }
        }
        public async Task<bool> CheckCenterAndResource(int centerId,int centerResourceId)
        {
            try {
            var resourceExists = await _db.CenterResources.AnyAsync(r => r.CenterResourceId == centerResourceId && r.CenterId == centerId);
                return resourceExists;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Check Center And Resource Error", ex);
                return false;
            }
        }
        // ================ Read By ===========
    }
}
