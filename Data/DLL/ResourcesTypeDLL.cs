using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class ResourcesTypeDLL
    {
        // ================ CRUD ===========
        public static async Task<int> Add(ResourcesType rt)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.ResourcesTypes.Add(rt);
                await db.SaveChangesAsync();
                return rt.ResourcesTypeId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add ResourcesType Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(ResourcesType rt)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.ResourcesTypes.FirstOrDefaultAsync(r => r.ResourcesTypeId == rt.ResourcesTypeId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(rt);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update ResourcesType Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.ResourcesTypes.FirstOrDefaultAsync(r => r.ResourcesTypeId == id);
                if (existing == null) return false;
                db.ResourcesTypes.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete ResourcesType Error", ex);
                return false;
            }
        }
        public static async Task<List<ResourcesType>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.ResourcesTypes.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All ResourcesTypes Error", ex);
                return new List<ResourcesType>();
            }
        }
        // ================ CRUD ===========


        // ================ Read By ===========
        public static async Task<ResourcesType?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.ResourcesTypes.AsNoTracking().FirstOrDefaultAsync(r => r.ResourcesTypeId == id);
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
