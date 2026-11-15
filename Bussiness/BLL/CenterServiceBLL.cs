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
        public async Task<bool> Add(CenterService service, int centerId)
        {
            int serviceID = await _serviceDLL.Add(service, centerId);
            return serviceID > 0;
        }
        public async Task<bool> Update(CenterService service)
            => await _serviceDLL.Update(service);
        public async Task<bool> ActiveStatus(int centerServiceId, int centerId)
        {
            var service = await GetByID(centerServiceId);

            if (service == null || service.CenterId != centerId)
                throw new KeyNotFoundException("Service not found.");

            if (service.IsActive)
                throw new ArgumentException("Center service already active.");

           return await _serviceDLL.ActiveStatus(centerServiceId, centerId); 
        }
        public async Task<bool> DeactiveStatus(int centerServiceId, int centerId)
        {
            var service = await GetByID(centerServiceId);

            if (service == null || service.CenterId != centerId)
                throw new KeyNotFoundException("Service not found.");

            if (!service.IsActive)
                throw new ArgumentException("Center service already deactive.");

          return await _serviceDLL.DeactiveStatus(centerServiceId, centerId);
        }
        public async Task<bool> Delete(int serviceId, int centerId)
            => await _serviceDLL.Delete(serviceId, centerId);
        public async Task<List<DTO_ServicesInfo>> GetAll()
            => await _serviceDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<CenterService?> GetByID(int centerServiceId)
            => await _serviceDLL.GetByID(centerServiceId);
        public async Task<List<DTO_ServicesInfo>?> GetAllByCenterId(int centerId)
            => await _serviceDLL.GetAllByCenterId(centerId);
        // ================ Read By ================
    }
}