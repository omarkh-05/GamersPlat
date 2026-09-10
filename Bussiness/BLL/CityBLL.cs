using Data;
using DataLayer;
namespace Bussiness
{
    public class CityBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private City _city;
        public int _cityID = -1;

        public CityBLL()
        {
            _city = new City();
            _mode = enMode.AddMode;
        }

        public CityBLL(City city)
        {
            _city = city;
            _mode = enMode.UpdateMode;
        }

        public City CurrentCity { get => _city; set => _city = value; }

        public bool Add()
        {
            _cityID = CityDLL.Add(_city);
            return _cityID > 0;
        }

        public bool Update() => CityDLL.Update(_city);

        public bool Delete(int id) => CityDLL.Delete(id);

        public static Task<City?> GetByID(int id) => CityDLL.GetByID(id);

        public static Task<List<City>> GetAll() => CityDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
