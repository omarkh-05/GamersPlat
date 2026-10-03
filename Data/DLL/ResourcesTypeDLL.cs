using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class ResourcesTypeDLL
    {
        private readonly GamersPlatDbContext _db;

        public ResourcesTypeDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        // ================ CRUD ===========
        public async Task<int> Add(ResourcesType rt)
        {
            try
            {
                _db.ResourcesTypes.Add(rt);
                await _db.SaveChangesAsync();
                return rt.ResourcesTypeId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add ResourcesType Error", ex);
                return 0;
            }
        }
        public async Task<bool> Update(ResourcesType rt)
        {
            try
            { 
                var existing = await _db.ResourcesTypes.FirstOrDefaultAsync(r => r.ResourcesTypeId == rt.ResourcesTypeId && r.CenterId == rt.CenterId);
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
        public async Task<bool> UpdateActiveStatus(int centerId,int resourceId)
        {
            try
            {

                var resource = await _db.ResourcesTypes
             .FirstOrDefaultAsync(r =>
                 r.ResourcesTypeId == resourceId &&
                 r.CenterId == centerId);


                if (resource == null)
                    return false;

                resource.IsActive = !resource.IsActive;

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
                var existing = await _db.ResourcesTypes.FirstOrDefaultAsync(r => r.ResourcesTypeId == id);
                if (existing == null) return false;
                _db.ResourcesTypes.Remove(existing);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete ResourcesType Error", ex);
                return false;
            }
        }
        public async Task<List<ResourcesType>> GetAll()
        {
            try
            {
                return await _db.ResourcesTypes.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All ResourcesTypes Error", ex);
                return new List<ResourcesType>();
            }
        }
        // ================ CRUD ===========


        // ================ Read By ===========
        public async Task<ResourcesType?> GetByID(int id)
        {
            try
            {
                return await _db.ResourcesTypes.AsNoTracking().FirstOrDefaultAsync(r => r.ResourcesTypeId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get ResourcesType By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ===========
    }
}
