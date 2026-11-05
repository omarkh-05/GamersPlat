using Data;
using DataLayer;
using Domain.DTOs.Service;

namespace Bussiness
{
    public class CenterServiceBLL
    {
        private readonly CenterServiceDLL _serviceDLL;

        public CenterServiceBLL(CenterServiceDLL serviceDLL)
        {
            _serviceDLL = serviceDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(Service service, int centerId)
        {
            int serviceID = await _serviceDLL.Add(service, centerId);
            return serviceID > 0;
        }
        public async Task<bool> Update(Service service)
            => await _serviceDLL.Update(service);
        public async Task<bool> ActiveStatus(int serviceId, int centerId)
            => await _serviceDLL.ActiveStatus(serviceId, centerId);
        public async Task<bool> DeactiveStatus(int serviceId, int centerId)
            => await _serviceDLL.DeactiveStatus(serviceId, centerId);
        public async Task<bool> Delete(int serviceId, int centerId)
            => await _serviceDLL.Delete(serviceId, centerId);
        public async Task<List<Service>> GetAll()
            => await _serviceDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Service?> GetByID(int serviceId)
            => await _serviceDLL.GetByID(serviceId);
        public async Task<List<DTO_ServicesInfo>?> GetAllByCenterId(int centerId)
            => await _serviceDLL.GetAllByCenterId(centerId);
        // ================ Read By ================
    }
}