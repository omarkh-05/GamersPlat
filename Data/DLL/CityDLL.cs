using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class CityDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(City city)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Cities.Add(city);
                await db.SaveChangesAsync();
                return city.CityId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add City Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Update(City city)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Cities.FirstOrDefaultAsync(c => c.CityId == city.CityId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(city);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update City Error", ex);
                return false;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Cities.FirstOrDefaultAsync(c => c.CityId == id);
                if (existing == null) return false;
                db.Cities.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete City Error", ex);
                return false;
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
                EventLog_Helper.WriteEventLog("Get All Cities Error", ex);
                return new List<City>();
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<City?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.CityId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get City By ID Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
