using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using Data.DLL;

namespace DataLayer
{
    public class CenterDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(Center center)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Centers.Add(center);
                db.SaveChanges();
                return center.CenterId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Center Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Center center)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Centers.FirstOrDefaultAsync(c => c.CenterId == center.CenterId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(center);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Center Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int centerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Centers.FirstOrDefaultAsync(c => c.CenterId == centerId);
                if (existing == null) return false;
                db.Centers.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Center Error", ex);
                return false;
            }
        }
        public static async Task<List<Center>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Centers
                    .Include(c => c.City)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Centers Error", ex);
                return new List<Center>();
            }
        }
        public static async Task<List<string>> GetCenterNames()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Centers
                    .Where(c => c.IsActive == true)
                    .Select(c => c.CenterName)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Center Names Error", ex);
                return new List<string>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<Center?> GetByID(int centerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Centers
                    .Include(c => c.City)
                    .Include(c => c.OwnerUser)
                    .Include(c => c.CenterImage)
                    .Include(c => c.Services)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CenterId == centerId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Center By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
