using Data;
using Domain.DTOs.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IResourcesTypeService
    {
        Task<bool> Add(CenterResource rt);
        Task<bool> Update(DTO_UpdateResource resource, int centerId, int centerResourceId);
        Task<bool> ActiveStatus(int centerId, int resourceId);
        Task<bool> DeactiveStatus(int centerId, int resourceId);
        Task<bool> Delete(int id);
        Task<List<DTO_ResourceInfo>> GetAll();
        Task<CenterResource?> GetByID(int id);
        Task<DTO_ResourceInfo?> GetResourceInfo(int resourcesTypeId);
        Task<List<DTO_ResourceInfo>> GetAllByCenterId(int centerId);
        Task<bool> CheckCenterAndResource(int centerId, int centerResourceId);
        int LastId { get; }
    }
}
