using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class ServiceDLL
    {
        public static int Add(Service service)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Services.Add(service);
                db.SaveChanges();
                return service.ServiceId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Service Error", ex);
                return 0;
            }
        }

        public static bool Update(Service service)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Services.FirstOrDefault(s => s.ServiceId == service.ServiceId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(service);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Service Error", ex);
                return false;
            }
        }

        public static bool Delete(int serviceId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Services.FirstOrDefault(s => s.ServiceId == serviceId);
                if (existing == null) return false;
                db.Services.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Service Error", ex);
                return false;
            }
        }

        public static async Task<Service?> GetByID(int serviceId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Services
                    .Include(s => s.Center)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Service By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<Service>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Services
                    .Include(s => s.Center)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Services Error", ex);
                return new List<Service>();
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
