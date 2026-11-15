using Bussiness.Interfaces;
using Data;
using DataLayer;
using Domain.DTOs.Resource;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bussiness
{
    public class CenterResourcesBLL : IResourcesTypeService
    {
        private readonly CenterResourcesDLL _centerResourcesDLL;
        readonly ICenterService _center;
        public int LastId => _rtID;
        public int _rtID { get; private set; }

        public CenterResourcesBLL(CenterResourcesDLL resourcesTypeDLL, ICenterService center)
        {
            _centerResourcesDLL = resourcesTypeDLL;
            _center = center;
        }

        // ================ CRUD ================
        public async Task<bool> Add(CenterResource rt)
        {
            int rtID = await _centerResourcesDLL.Add(rt);
            _rtID = rtID;
            return rtID > 0;
        }
        public async Task<bool> Update(DTO_UpdateResource resource, int centerId, int centerResourceId)
         {
            var existingCenterResource = await _centerResourcesDLL.GetByID(centerResourceId);

            if (existingCenterResource == null)
                throw new KeyNotFoundException("Resource not found.");

            if (existingCenterResource.CenterId != centerId)
                throw new UnauthorizedAccessException("Resource does not belong to this center.");

            existingCenterResource.ResourcesTypeId = resource.ResourceId;
            existingCenterResource.HourlyPrice = resource.HourlyPrice;
            existingCenterResource.TotalQuantity = resource.TotalQuantity;
            existingCenterResource.RoomType = resource.RoomType ?? "normal";
            existingCenterResource.Description = resource.Description ?? string.Empty;
            existingCenterResource.IsActive = resource.IsActive;
            existingCenterResource.UpdatedAt = DateTime.Now;

           return await _centerResourcesDLL.Update(existingCenterResource);
        }
        public async Task<bool> ActiveStatus(int centerId, int centerResourceId)
         {
            var resource = await GetByID(centerResourceId);

            if (resource == null || resource.CenterId != centerId)
                throw new KeyNotFoundException("Resource not found.");

            if (resource.IsActive)
                throw new ArgumentException("Center resource already active.");

           return await _centerResourcesDLL.ActiveStatus(centerId, centerResourceId);
        }
        public async Task<bool> DeactiveStatus(int centerId, int centerResourceId)
        {
            var resource = await GetByID(centerResourceId);

            if (resource == null || resource.CenterId != centerId)
                throw new KeyNotFoundException("Resource not found.");

            if (!resource.IsActive)
                throw new ArgumentException("Center resource already deactive.");

            return await _centerResourcesDLL.DeactiveStatus(centerId, centerResourceId);
        }
        public async Task<bool> Delete(int id)
            => await _centerResourcesDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<DTO_ResourceInfo>> GetAll()
            => await _centerResourcesDLL.GetAll();
        public async Task<CenterResource?> GetByID(int id)
            => await _centerResourcesDLL.GetByID(id);
        public async Task<List<DTO_ResourceInfo>> GetAllByCenterId(int centerId)
            => await _centerResourcesDLL.GetAllByCenterId(centerId);
        public async Task<DTO_ResourceInfo?> GetResourceInfo(int resourcesTypeId)
        { 
            var resInfo = await _centerResourcesDLL.GetByID(resourcesTypeId);
            var dto = new DTO_ResourceInfo
            {
                ResourcesTypeId = resInfo?.ResourcesTypeId ?? 0,
                CenterId = resInfo?.CenterId ?? 0,
                HourlyPrice = resInfo?.HourlyPrice ?? 0,
                TotalQuantity = resInfo?.TotalQuantity ?? 0,
                RoomType = resInfo?.RoomType ?? string.Empty,
                IsActive = resInfo?.IsActive ?? false
            };
            return dto;
        }
        public async Task<bool> CheckCenterAndResource(int centerId, int resourceId)
        {
            var resInfo = await _centerResourcesDLL.CheckCenterAndResource(centerId, resourceId);
            return resInfo;
        }
        // ================ Read By ================
    }
}