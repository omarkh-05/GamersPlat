using Data;
using DataLayer;
namespace Bussiness
{
    public class ResourcesTypeBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(ResourcesType rt)
        {
            int rtID = await ResourcesTypeDLL.Add(rt);
            return rtID > 0;
        }
        public async Task<bool> Update(ResourcesType rt) => await ResourcesTypeDLL.Update(rt);
        public async Task<bool> Delete(int id) => await ResourcesTypeDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<ResourcesType>> GetAll() => await ResourcesTypeDLL.GetAll();
        public async Task<ResourcesType?> GetByID(int id) => await ResourcesTypeDLL.GetByID(id);
        // ================ Read By ================
    }
}
