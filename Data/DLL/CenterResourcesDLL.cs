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
                var existing = await _db.CenterResources.FirstOrDefaultAsync(r => r.ResourcesTypeId == rt.ResourcesTypeId && r.CenterId == rt.CenterId);
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
        public async Task<bool> ActiveStatus(int centerId,int resourceId)
        {
            try
            {

                var resource = await _db.CenterResources
             .FirstOrDefaultAsync(r =>
                 r.ResourcesTypeId == resourceId &&
                 r.CenterId == centerId);


                if (resource == null)
                    return false;

                resource.IsActive = true;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update ResourcesType Active Status Error", ex);
                return false;
            }
        }
        public async Task<bool> DeactiveStatus(int centerId, int resourceId)
        {
            try
            {

                var resource = await _db.CenterResources
             .FirstOrDefaultAsync(r =>
                 r.ResourcesTypeId == resourceId &&
                 r.CenterId == centerId);


                if (resource == null)
                    return false;

                resource.IsActive = false;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update ResourcesType Active Status Error", ex);
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
        public async Task<List<CenterResource>> GetAll()
        {
            try
            {
                return await _db.CenterResources.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Center Resources Error", ex);
                return new List<CenterResource>();
            }
        }
        // ================ CRUD ===========


        // ================ Read By ===========
        public async Task<CenterResource?> GetByID(int id)
        {
            try
            {
                return await _db.CenterResources.AsNoTracking().FirstOrDefaultAsync(r => r.ResourcesTypeId == id);
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
                        ResourcesTypeId = r.ResourcesTypeId,
                        CenterId = r.CenterId,
                        ResourceId = r.ResourcesTypeId,
                        HourlyPrice = r.HourlyPrice,
                        TotalQuantity = r.TotalQuantity,
                        RoomType = r.RoomType,
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
        public async Task<bool> CheckCenterAndResource(int centerId,int resourceId)
        {
            try {
            var resourceExists = await _db.CenterResources.AnyAsync(r => r.ResourcesTypeId == resourceId && r.CenterId == centerId);
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
