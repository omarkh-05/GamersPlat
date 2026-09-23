using Bussiness.Interfaces;
using Data;
using DataLayer;
using Domain.DTOs.Center;
namespace Bussiness
{
    public class CenterBLL
    {
        public int _centerID { get; private set; }
        // ================ CRUD ================
        public async Task<bool> Add(DTO_CreateCenter addCenter)
        {
            if(addCenter == null) throw new ArgumentNullException(nameof(addCenter), "Center data cannot be null.");

            var createCenter = new Center
            {
                OwnerUserId = addCenter.OwnerUserId <= 0 ? throw new ArgumentException("Invalid owner ID.") : addCenter.OwnerUserId,
                CenterName = addCenter.CenterName,
                CityId = addCenter.CityId <= 0 ? throw new ArgumentException("Invalid City ID.") : addCenter.CityId,
                CenterAddress = addCenter.CenterAddress,
                CenterDescription = addCenter.CenterDescription,
                CenterStatus = addCenter.CenterStatus,
                IsActive = addCenter.IsActive,
                CenterType = addCenter.CenterType,
                OpenTime = addCenter.OpenTime <= TimeOnly.MinValue ? throw new ArgumentException("Invalid open time.") : addCenter.OpenTime,
                CloseTime = addCenter.CloseTime <= TimeOnly.MinValue ? throw new ArgumentException("Invalid close time.") : addCenter.CloseTime,
                CreatedAt = DateTime.UtcNow
            };
            int centerID = await CenterDLL.Add(createCenter);
            _centerID = centerID;
            return centerID > 0;
        }
        public async Task<bool> Update(DTO_UpdateCenter center, int ownerId)
        {
            if (center == null) throw new ArgumentNullException(nameof(center), "Center data cannot be null.");
            Center updateCenter = new Center
            {
                CenterId = center.CenterId,
                CenterName = center.CenterName,
                CityId = center.CityId,
                CenterAddress = center.CenterAddress,
                CenterDescription = center.CenterDescription,
                CenterStatus = center.CenterStatus,
                IsActive = center.IsActive,
                CenterType = center.CenterType,
                OpenTime = center.OpenTime,
                CloseTime = center.CloseTime
            };
            await CenterDLL.Update(updateCenter, ownerId);

            return true;
        }
        public async Task<bool> UpdateActiveStatus(int centerId, int ownerId) => await CenterDLL.UpdateActiveStatus(centerId, ownerId);
        public async Task<bool> ToggleCenterStatus(int centerId, string status, int ownerId) => await CenterDLL.ToggleCenterStatus(centerId, status, ownerId);
        public async Task<bool> Delete(int centerId) => await CenterDLL.Delete(centerId);
        public async Task<List<DTO_HomePageCentersDetails>> GetAll() => await CenterDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Center?> GetByID(int centerId) {
            if(centerId <= 0) throw new ArgumentException("Invalid center ID.");
            return await CenterDLL.GetByID(centerId);
        }
        public async Task<DTO_CenterDetails?> GetCenterDetailsById(int centerId)
        {
            if (centerId <= 0) throw new ArgumentException("Invalid center ID.");
            return await CenterDLL.GetCenterDetailsById(centerId);
        }
        public async Task<List<DTO_CentersListDetails>> GetAllCentersByOwnerId(int ownerId)
        {
            if (ownerId <= 0) throw new ArgumentException("Invalid owner ID.");
            return await CenterDLL.GetAllCentersByOwnerId(ownerId);
        }
        public async Task<List<string>> GetCenterNames() => await CenterDLL.GetCenterNames();
        // ================ Read By ================
    }
}
