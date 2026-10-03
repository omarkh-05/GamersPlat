using Data;
using DataLayer;
using Bussiness.Interfaces;

namespace Bussiness
{
    public class ResourcesTypeBLL : IResourcesTypeService
    {
        private readonly ResourcesTypeDLL _resourcesTypeDLL;

        public int LastId => _rtID;
        public int _rtID { get; private set; }

        public ResourcesTypeBLL(ResourcesTypeDLL resourcesTypeDLL)
        {
            _resourcesTypeDLL = resourcesTypeDLL;
        }

        // ================ CRUD ================

        public async Task<bool> Add(ResourcesType rt)
        {
            int rtID = await _resourcesTypeDLL.Add(rt);
            _rtID = rtID;
            return rtID > 0;
        }

        public async Task<bool> Update(ResourcesType rt)
            => await _resourcesTypeDLL.Update(rt);

        public async Task<bool> UpdateActiveStatus(int centerId, int resourceId)
            => await _resourcesTypeDLL.UpdateActiveStatus(centerId, resourceId);

        public async Task<bool> Delete(int id)
            => await _resourcesTypeDLL.Delete(id);

        // ================ CRUD ================


        // ================ Read By ================

        public async Task<List<ResourcesType>> GetAll()
            => await _resourcesTypeDLL.GetAll();

        public async Task<ResourcesType?> GetByID(int id)
            => await _resourcesTypeDLL.GetByID(id);

        // ================ Read By ================
    }
}