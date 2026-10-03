using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class CityDLL
    {
        private readonly GamersPlatDbContext _db;

        public CityDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(City city)
        {
            try
            {
                _db.Cities.Add(city);
                await _db.SaveChangesAsync();
                return city.CityId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add City Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(City city)
        {
            try
            {
                var existing = await _db.Cities
                    .FirstOrDefaultAsync(c => c.CityId == city.CityId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(city);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update City Error", ex);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.Cities
                    .FirstOrDefaultAsync(c => c.CityId == id);

                if (existing == null)
                    return false;

                _db.Cities.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete City Error", ex);
                return false;
            }
        }

        public async Task<List<City>> GetAll()
        {
            try
            {
                return await _db.Cities
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Cities Error", ex);
                return new List<City>();
            }
        }

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<City?> GetByID(int id)
        {
            try
            {
                return await _db.Cities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CityId == id);
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