using Data;
using DataLayer;
namespace Bussiness
{
    public class CountryBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Country _country;
        public int _countryID = -1;

        public CountryBLL()
        {
            _country = new Country();
            _mode = enMode.AddMode;
        }

        public CountryBLL(Country country)
        {
            _country = country;
            _mode = enMode.UpdateMode;
        }

        public Country CurrentCountry { get => _country; set => _country = value; }

        public bool Add()
        {
            _countryID = CountryDLL.Add(_country);
            return _countryID > 0;
        }

        public bool Update() => CountryDLL.Update(_country);

        public bool Delete(int id) => CountryDLL.Delete(id);

        public static Task<Country?> GetByID(int id) => CountryDLL.GetByID(id);

        public static Task<List<Country>> GetAll() => CountryDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
