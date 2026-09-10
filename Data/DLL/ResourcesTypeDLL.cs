using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class ResourcesTypeDLL
    {
        public static int Add(ResourcesType rt)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.ResourcesTypes.Add(rt);
                db.SaveChanges();
                return rt.ResourcesTypeId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add ResourcesType Error", ex);
                return 0;
            }
        }

        public static bool Update(ResourcesType rt)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.ResourcesTypes.FirstOrDefault(r => r.ResourcesTypeId == rt.ResourcesTypeId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(rt);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update ResourcesType Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.ResourcesTypes.FirstOrDefault(r => r.ResourcesTypeId == id);
                if (existing == null) return false;
                db.ResourcesTypes.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete ResourcesType Error", ex);
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
                WriteEventLog("Get All ResourcesTypes Error", ex);
                return new List<ResourcesType>();
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
