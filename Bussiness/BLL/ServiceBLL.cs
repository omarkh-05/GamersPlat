using Data;
using DataLayer;
namespace Bussiness
{
    public class ServiceBLL
    {
        // ================ CRUD ================
        public bool Add(Service service)
        {
            int serviceID = ServiceDLL.Add(service);
            return serviceID > 0;
        }
        public async Task<bool> Update(Service service) => await ServiceDLL.Update(service);
        public async Task<bool> Delete(int serviceId) => await ServiceDLL.Delete(serviceId);
        public async Task<List<Service>> GetAll() => await ServiceDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Service?> GetByID(int serviceId) => await ServiceDLL.GetByID(serviceId);
        // ================ Read By ================
    }
}
