using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.City;
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

        public async Task<List<DTO_AllCities>> GetAll()
        {
            try
            {
                return await _db.Cities
                    .AsNoTracking()
                    .Select(c => new DTO_AllCities
                    {
                        CityId = c.CityId,
                        Name = c.Name
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Cities Error", ex);
                return new List<DTO_AllCities>();
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
        public async Task<int> GetIdByName(string name)
        {
            try
            {
                return await _db.Cities
                    .AsNoTracking()
                    .Where(c=>c.Name == name)
                    .Select(c => c.CityId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get City ID By Name Error", ex);
                return -1;
            }
        }
        public async Task<bool> CityExist(int id)
        {
            try
            {
                return await _db.Cities.AnyAsync(c => c.CityId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get City By ID Error", ex);
                return false;
            }
        }
        // ================ Read By ================
    }
}