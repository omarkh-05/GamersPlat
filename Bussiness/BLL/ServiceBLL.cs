using Data;
using DataLayer;

namespace Bussiness
{
    public class ServiceBLL
    {
        private readonly ServiceDLL _serviceDLL;

        public ServiceBLL(ServiceDLL serviceDLL)
        {
            _serviceDLL = serviceDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(Service service)
        {
            int serviceID = await _serviceDLL.Add(service);
            return serviceID > 0;
        }

        public async Task<bool> Update(Service service)
            => await _serviceDLL.Update(service);

        public async Task<bool> UpdateActiveStatus(int serviceId, int centerId)
            => await _serviceDLL.UpdateActiveStatus(serviceId, centerId);

        public async Task<bool> Delete(int serviceId, int centerId)
            => await _serviceDLL.Delete(serviceId, centerId);

        public async Task<List<Service>> GetAll()
            => await _serviceDLL.GetAll();

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Service?> GetByID(int serviceId)
            => await _serviceDLL.GetByID(serviceId);

        // ================ Read By ================
    }
}