using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IResourcesTypeService
    {
        Task<bool> Add(ResourcesType rt);
        Task<bool> Update(ResourcesType rt);
        Task<bool> UpdateActiveStatus(int centerId, int resourceId);
        Task<bool> Delete(int id);
        Task<List<ResourcesType>> GetAll();
        Task<ResourcesType?> GetByID(int id);
        int LastId { get; }
    }
}
