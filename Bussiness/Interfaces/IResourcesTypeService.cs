using Data;
using Domain.DTOs.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IResourcesTypeService
    {
        Task<bool> Add(ResourcesType rt);
        Task<bool> Update(ResourcesType rt);
        Task<bool> ActiveStatus(int centerId, int resourceId);
        Task<bool> DeactiveStatus(int centerId, int resourceId);
        Task<bool> Delete(int id);
        Task<List<ResourcesType>> GetAll();
        Task<ResourcesType?> GetByID(int id);
        Task<DTO_ResourceInfo?> GetResourceInfo(int resourcesTypeId);
        Task<List<DTO_ResourceInfo>> GetAllByCenterId(int centerId);
        Task<bool> CheckCenterAndResource(int centerId, int resourceId);
        int LastId { get; }
    }
}
