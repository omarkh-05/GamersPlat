using Data;
using DataLayer;

namespace Bussiness
{
    public class DeviceBLL
    {
        private readonly DeviceDLL _deviceDLL;

        public DeviceBLL(DeviceDLL deviceDLL)
        {
            _deviceDLL = deviceDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(Device device)
        {
            int deviceID = await _deviceDLL.Add(device);
            return deviceID > 0;
        }

        public async Task<bool> Update(Device device)
            => await _deviceDLL.Update(device);

        public async Task<bool> Delete(int id)
            => await _deviceDLL.Delete(id);

        public async Task<List<Device>> GetAll()
            => await _deviceDLL.GetAll();

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Device?> GetByID(int id)
            => await _deviceDLL.GetByID(id);

        // ================ Read By ================
    }
}