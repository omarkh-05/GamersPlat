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
        private readonly CenterResourcesDLL _resourcesTypeDLL;

        public int LastId => _rtID;
        public int _rtID { get; private set; }

        public CenterResourcesBLL(CenterResourcesDLL resourcesTypeDLL)
        {
            _resourcesTypeDLL = resourcesTypeDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(CenterResource rt)
        {
            int rtID = await _resourcesTypeDLL.Add(rt);
            _rtID = rtID;
            return rtID > 0;
        }
        public async Task<bool> Update(CenterResource rt)
            => await _resourcesTypeDLL.Update(rt);
        public async Task<bool> ActiveStatus(int centerId, int resourceId)
            => await _resourcesTypeDLL.ActiveStatus(centerId, resourceId);
        public async Task<bool> DeactiveStatus(int centerId, int resourceId)
            => await _resourcesTypeDLL.DeactiveStatus(centerId, resourceId);
        public async Task<bool> Delete(int id)
            => await _resourcesTypeDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<CenterResource>> GetAll()
            => await _resourcesTypeDLL.GetAll();
        public async Task<CenterResource?> GetByID(int id)
            => await _resourcesTypeDLL.GetByID(id);
        public async Task<List<DTO_ResourceInfo>> GetAllByCenterId(int centerId)
            => await _resourcesTypeDLL.GetAllByCenterId(centerId);
        public async Task<DTO_ResourceInfo?> GetResourceInfo(int resourcesTypeId)
        { 
            var resInfo = await _resourcesTypeDLL.GetByID(resourcesTypeId);
            var dto = new DTO_ResourceInfo
            {
                ResourcesTypeId = resInfo?.ResourcesTypeId ?? 0,
                CenterId = resInfo?.CenterId ?? 0,
                ResourceId = resInfo?.ResourcesTypeId ?? 0,
                HourlyPrice = resInfo?.HourlyPrice ?? 0,
                TotalQuantity = resInfo?.TotalQuantity ?? 0,
                RoomType = resInfo?.RoomType ?? string.Empty,
                IsActive = resInfo?.IsActive ?? false
            };
            return dto;
        }
        public async Task<bool> CheckCenterAndResource(int centerId, int resourceId)
        {
            var resInfo = await _resourcesTypeDLL.CheckCenterAndResource(centerId, resourceId);
            return resInfo;
        }
        // ================ Read By ================
    }
}