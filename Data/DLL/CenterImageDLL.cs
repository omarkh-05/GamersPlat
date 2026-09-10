using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class CenterImageDLL
    {
        public static int Add(CenterImage img)
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
                WriteEventLog("Add CenterImage Error", ex);
                return 0;
            }
        }

        public static bool Update(CenterImage img)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.CenterImages.FirstOrDefault(i => i.ImageId == img.ImageId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(img);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update CenterImage Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.CenterImages.FirstOrDefault(i => i.ImageId == id);
                if (existing == null) return false;
                db.CenterImages.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete CenterImage Error", ex);
                return false;
            }
        }

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
                WriteEventLog("Get CenterImage By ID Error", ex);
                return null;
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
                WriteEventLog("Get All CenterImages Error", ex);
                return new List<CenterImage>();
            }
        }

        private static void WriteEventLog(string title, Exception ex)
        {
            string error = ex.Message;
            if (ex.InnerException != null)
                error += "\nInner Exception: " + ex.InnerException.Message;
            EventLog.WriteEntry("Application", $"{title}: {error}", EventLogEntryType.Error);
        }
    }
}
