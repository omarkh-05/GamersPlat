using Data;
using DataLayer;
using Domain.DTOs.City;

namespace Bussiness
{
    public class CityBLL
    {
        private readonly CityDLL _cityDLL;

        public CityBLL(CityDLL cityDLL)
        {
            _cityDLL = cityDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(City city)
        {
            int cityID = await _cityDLL.Add(city);
            return cityID > 0;
        }

        public async Task<bool> Update(City city)
            => await _cityDLL.Update(city);

        public async Task<bool> Delete(int id)
            => await _cityDLL.Delete(id);

        public async Task<List<DTO_AllCities>> GetAll()
            => await _cityDLL.GetAll();

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<City?> GetByID(int id)
            => await _cityDLL.GetByID(id);
        public async Task<int> GetIdByName(string name) => await _cityDLL.GetIdByName(name);
        public async Task<bool> CityExist(int id)
            => await _cityDLL.CityExist(id);
        // ================ Read By ================
    }
}