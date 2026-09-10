using Data;
using DataLayer;
namespace Bussiness
{
    public class DeviceBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Device _device;
        public int _deviceID = -1;

        public DeviceBLL()
        {
            _device = new Device();
            _mode = enMode.AddMode;
        }

        public DeviceBLL(Device device)
        {
            _device = device;
            _mode = enMode.UpdateMode;
        }

        public Device CurrentDevice { get => _device; set => _device = value; }

        public bool Add()
        {
            _deviceID = DeviceDLL.Add(_device);
            return _deviceID > 0;
        }

        public bool Update() => DeviceDLL.Update(_device);

        public bool Delete(int id) => DeviceDLL.Delete(id);

        public static Task<Device?> GetByID(int id) => DeviceDLL.GetByID(id);

        public static Task<List<Device>> GetAll() => DeviceDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
