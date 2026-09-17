using Data;
using DataLayer;
namespace Bussiness
{
    public class ServiceBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(Service service)
        {
            int serviceID = await ServiceDLL.Add(service);
            return serviceID > 0;
        }
        public async Task<bool> Update(Service service) => await ServiceDLL.Update(service);
        public async Task<bool> UpdateActiveStatus(int serviceId, int centerId) => await ServiceDLL.UpdateActiveStatus(serviceId, centerId);
        public async Task<bool> Delete(int serviceId,int centerId) => await ServiceDLL.Delete(serviceId, centerId);
        public async Task<List<Service>> GetAll() => await ServiceDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Service?> GetByID(int serviceId) => await ServiceDLL.GetByID(serviceId);
        // ================ Read By ================
    }
}
