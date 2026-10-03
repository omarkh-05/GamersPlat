using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class CountryDLL
    {
        private readonly GamersPlatDbContext _db;

        public CountryDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(Country country)
        {
            try
            {
                _db.Countries.Add(country);
                await _db.SaveChangesAsync();
                return country.CountryId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Country Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(Country country)
        {
            try
            {
                var existing = await _db.Countries
                    .FirstOrDefaultAsync(c => c.CountryId == country.CountryId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(country);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Country Error", ex);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.Countries
                    .FirstOrDefaultAsync(c => c.CountryId == id);

                if (existing == null)
                    return false;

                _db.Countries.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Country Error", ex);
                return false;
            }
        }

        public async Task<List<Country>> GetAll()
        {
            try
            {
                return await _db.Countries
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Countries Error", ex);
                return new List<Country>();
            }
        }

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Country?> GetByID(int id)
        {
            try
            {
                return await _db.Countries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CountryId == id);
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