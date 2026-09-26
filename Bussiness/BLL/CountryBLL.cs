using Data;
using DataLayer;
namespace Bussiness
{
    public class CountryBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(Country country)
        {
            int countryID = await CountryDLL.Add(country);
            return countryID > 0;
        }
        public async Task<bool> Update(Country country) => await CountryDLL.Update(country);
        public async Task<bool> Delete(int id) => await CountryDLL.Delete(id);
        public async Task<List<Country>> GetAll() => await CountryDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Country?> GetByID(int id) => await CountryDLL.GetByID(id);
        // ================ Read By ================
    }
}
