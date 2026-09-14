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
        public bool Update(City city) => CityDLL.Update(city);
        public bool Delete(int id) => CityDLL.Delete(id);
        public static Task<List<City>> GetAll() => CityDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<City?> GetByID(int id) => CityDLL.GetByID(id);
        // ================ Read By ================
    }
}
