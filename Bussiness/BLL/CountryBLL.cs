using Data;
using DataLayer;
namespace Bussiness
{
    public class CountryBLL
    {
        // ================ CRUD ================
        public bool Add(Country country)
        {
            int countryID = CountryDLL.Add(country);
            return countryID > 0;
        }
        public bool Update(Country country) => CountryDLL.Update(country);
        public bool Delete(int id) => CountryDLL.Delete(id);
        public static Task<List<Country>> GetAll() => CountryDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<Country?> GetByID(int id) => CountryDLL.GetByID(id);
        // ================ Read By ================
    }
}
