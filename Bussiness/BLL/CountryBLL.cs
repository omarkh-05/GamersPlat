using Data;
using DataLayer;

namespace Bussiness
{
    public class CountryBLL
    {
        private readonly CountryDLL _countryDLL;

        public CountryBLL(CountryDLL countryDLL)
        {
            _countryDLL = countryDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(Country country)
        {
            int countryID = await _countryDLL.Add(country);
            return countryID > 0;
        }

        public async Task<bool> Update(Country country)
            => await _countryDLL.Update(country);

        public async Task<bool> Delete(int id)
            => await _countryDLL.Delete(id);

        public async Task<List<Country>> GetAll()
            => await _countryDLL.GetAll();

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Country?> GetByID(int id)
            => await _countryDLL.GetByID(id);

        // ================ Read By ================
    }
}