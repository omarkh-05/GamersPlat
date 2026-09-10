using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class CountryDLL
    {
        public static int Add(Country country)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Countries.Add(country);
                db.SaveChanges();
                return country.CountryId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add Country Error", ex);
                return 0;
            }
        }

        public static bool Update(Country country)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Countries.FirstOrDefault(c => c.CountryId == country.CountryId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(country);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Country Error", ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Countries.FirstOrDefault(c => c.CountryId == id);
                if (existing == null) return false;
                db.Countries.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete Country Error", ex);
                return false;
            }
        }

        public static async Task<Country?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.CountryId == id);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get Country By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<Country>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Countries.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Countries Error", ex);
                return new List<Country>();
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
