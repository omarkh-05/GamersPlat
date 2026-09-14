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
        public bool Update(Service service) => ServiceDLL.Update(service);
        public bool Delete(int serviceId) => ServiceDLL.Delete(serviceId);
        public Task<List<Service>> GetAll() => ServiceDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public Task<Service?> GetByID(int serviceId) => ServiceDLL.GetByID(serviceId);
        // ================ Read By ================
    }
}
