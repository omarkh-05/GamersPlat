using Data;
using DataLayer;
namespace Bussiness
{
    public class CityBLL
    {
        // ================ CRUD ================
        public bool Add(City city)
        {
            int cityID = CityDLL.Add(city);
            return cityID > 0;
        }
        public async Task<bool> Update(City city) => await CityDLL.Update(city);
        public async Task<bool> Delete(int id) => await CityDLL.Delete(id);
        public async Task<List<City>> GetAll() => await CityDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<City?> GetByID(int id) => await CityDLL.GetByID(id);
        // ================ Read By ================
    }
}
