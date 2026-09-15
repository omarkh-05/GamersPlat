using Data;
using DataLayer;
namespace Bussiness
{
    public class DeviceBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(Device device)
        {
            int deviceID = await DeviceDLL.Add(device);
            return deviceID > 0;
        }
        public async Task<bool> Update(Device device) => await DeviceDLL.Update(device);
        public async Task<bool> Delete(int id) => await DeviceDLL.Delete(id);
        public static async Task<List<Device>> GetAll() => await DeviceDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<Device?> GetByID(int id) => await DeviceDLL.GetByID(id);
        // ================ Read By ================
    }
}
