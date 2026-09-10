using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class CityDLL
    {
        public static int Add(City city)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Cities.Add(city);
                db.SaveChanges();
                return city.CityId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add City Error", ex);
                return 0;
            }
        }

        public static bool Update(City city)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Cities.FirstOrDefault(c => c.CityId == city.CityId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(city);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update City Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Cities.FirstOrDefault(c => c.CityId == id);
                if (existing == null) return false;
                db.Cities.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete City Error", ex);
                return false;
            }
        }

        public static async Task<City?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.CityId == id);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get City By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<City>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Cities.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Cities Error", ex);
                return new List<City>();
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
