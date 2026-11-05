using Data;
using Domain.DTOs.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IResourcesTypeService
    {
        Task<bool> Add(CenterResource rt);
        Task<bool> Update(CenterResource rt);
        Task<bool> ActiveStatus(int centerId, int resourceId);
        Task<bool> DeactiveStatus(int centerId, int resourceId);
        Task<bool> Delete(int id);
        Task<List<CenterResource>> GetAll();
        Task<CenterResource?> GetByID(int id);
        Task<DTO_ResourceInfo?> GetResourceInfo(int resourcesTypeId);
        Task<List<DTO_ResourceInfo>> GetAllByCenterId(int centerId);
        Task<bool> CheckCenterAndResource(int centerId, int resourceId);
        int LastId { get; }
    }
}
