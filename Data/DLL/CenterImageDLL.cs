using Microsoft.EntityFrameworkCore;
using Data;
using Data.DLL;
using Data.EF;

namespace DataLayer
{
    public class CenterImageDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(CenterImage img)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.CenterImages.Add(img);
                db.SaveChanges();
                return img.ImageId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add CenterImage Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(CenterImage img)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.CenterImages.FirstOrDefaultAsync(i => i.ImageId == img.ImageId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(img);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update CenterImage Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.CenterImages.FirstOrDefaultAsync(i => i.ImageId == id);
                if (existing == null) return false;
                db.CenterImages.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete CenterImage Error", ex);
                return false;
            }
        }
        public static async Task<List<CenterImage>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.CenterImages.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All CenterImages Error", ex);
                return new List<CenterImage>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<CenterImage?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.CenterImages
                    .Include(i => i.Center)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.ImageId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get CenterImage By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
