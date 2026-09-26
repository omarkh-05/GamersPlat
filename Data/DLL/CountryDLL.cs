using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class CountryDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(Country country)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Countries.Add(country);
                await db.SaveChangesAsync();
                return country.CountryId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Country Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(Country country)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Countries.FirstOrDefaultAsync(c => c.CountryId == country.CountryId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(country);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Country Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Countries.FirstOrDefaultAsync(c => c.CountryId == id);
                if (existing == null) return false;
                db.Countries.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Country Error", ex);
                return false;
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
                EventLog_Helper.WriteEventLog("Get All Countries Error", ex);
                return new List<Country>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<Country?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.CountryId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Country By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
