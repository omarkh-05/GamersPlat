using Data;
using Data.DLL;
using Domain.DTOs.Resource;

namespace Bussiness.BLL
{
    public class ResourceBLL
    {
        readonly ResourceDLL _resource;
       public ResourceBLL(ResourceDLL resource)
        {
            _resource = resource;
        }

        // ================ CRUD ================
        public async Task<List<DTO_Resources>> GetAll()
        {
            var resources = await _resource.GetAll();
            if (resources == null || resources.Count == 0)
            {
                throw new InvalidOperationException("No resources found.");
            }
            return resources;
        }
        // ================ CRUD ================
    }
}
