using Data;
using DataLayer;
namespace Bussiness
{
    public class CenterBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Center _center;
        public int _centerID = -1;

        public CenterBLL()
        {
            _center = new Center();
            _mode = enMode.AddMode;
        }

        public CenterBLL(Center center)
        {
            _center = center;
            _mode = enMode.UpdateMode;
        }

        public Center CurrentCenter { get => _center; set => _center = value; }

        public bool Add()
        {
            _centerID = CenterDLL.Add(_center);
            return _centerID > 0;
        }

        public bool Update() => CenterDLL.Update(_center);

        public bool Delete(int centerId) => CenterDLL.Delete(centerId);

        public static Task<Center?> GetByID(int centerId) => CenterDLL.GetByID(centerId);

        public static Task<List<Center>> GetAll() => CenterDLL.GetAll();

        public static Task<List<string>> GetCenterNames() => CenterDLL.GetCenterNames();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
