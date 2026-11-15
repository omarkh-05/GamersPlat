using Bussiness.Interfaces;
using Data;
using Data.DLL;
using Domain.DTOs.Service;

namespace Bussiness.BLL
{
    public class ServiceBLL
    {
        readonly ServiceDLL _service;

        public ServiceBLL(ServiceDLL service)
        {
            _service = service;
        }

        // ================ CRUD ================
        public async Task<int> Add(DTO_AddService service) => await _service.Add(service.Name);
        public async Task<bool> Update(DTO_UpdateService service,byte serviceId) => await _service.Update(service.Name, serviceId);
        public async Task<bool> Delete(byte serviceId) => await _service.Delete(serviceId);
        public async Task<List<DTO_Services>> GetAll()
        {
            var resources = await _service.GetAll();
            if (resources == null || resources.Count == 0)
            {
                throw new InvalidOperationException("No services found.");
            }
            return resources;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Service?> GetById(byte serviceId) => await _service.GetById(serviceId);
        // ================ Read By ================

    }
}
