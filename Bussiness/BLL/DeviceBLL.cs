using Data;
using DataLayer;
namespace Bussiness
{
    public class DeviceBLL
    {
        // ================ CRUD ================
        public bool Add(Device device)
        {
            int deviceID = DeviceDLL.Add(device);
            return deviceID > 0;
        }
        public bool Update(Device device) => DeviceDLL.Update(device);
        public bool Delete(int id) => DeviceDLL.Delete(id);
        public static Task<List<Device>> GetAll() => DeviceDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<Device?> GetByID(int id) => DeviceDLL.GetByID(id);
        // ================ Read By ================
    }
}
