using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class CenterDLL
    {
        // ================ CRUD ================
        public static int Add(Center center)
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
                WriteEventLog("Add Center Error", ex);
                return 0;
            }
        }

        public static bool Update(Center center)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Centers.FirstOrDefault(c => c.CenterId == center.CenterId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(center);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Center Error", ex);
                return false;
            }
        }

        public static bool Delete(int centerId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Centers.FirstOrDefault(c => c.CenterId == centerId);
                if (existing == null) return false;
                db.Centers.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Center Error", ex);
                return false;
            }
        }

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
                WriteEventLog("Get Center By ID Error", ex);
                return null;
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
                WriteEventLog("Get All Centers Error", ex);
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
                WriteEventLog("Get Center Names Error", ex);
                return new List<string>();
            }
        }

        // ===================== EventLog Helper =====================
        private static void WriteEventLog(string title, Exception ex)
        {
            string error = ex.Message;
            if (ex.InnerException != null)
                error += "\nInner Exception: " + ex.InnerException.Message;

            EventLog.WriteEntry("Application", $"{title}: {error}", EventLogEntryType.Error);
        }
    }
}
