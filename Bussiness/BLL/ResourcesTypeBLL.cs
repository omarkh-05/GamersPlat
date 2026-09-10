using Data;
using DataLayer;
namespace Bussiness
{
    public class ResourcesTypeBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private ResourcesType _rt;
        public int _rtID = -1;

        public ResourcesTypeBLL()
        {
            _rt = new ResourcesType();
            _mode = enMode.AddMode;
        }

        public ResourcesTypeBLL(ResourcesType rt)
        {
            _rt = rt;
            _mode = enMode.UpdateMode;
        }

        public ResourcesType CurrentResourcesType { get => _rt; set => _rt = value; }

        public bool Add()
        {
            _rtID = ResourcesTypeDLL.Add(_rt);
            return _rtID > 0;
        }

        public bool Update() => ResourcesTypeDLL.Update(_rt);

        public bool Delete(int id) => ResourcesTypeDLL.Delete(id);

        public static Task<List<ResourcesType>> GetAll() => ResourcesTypeDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
