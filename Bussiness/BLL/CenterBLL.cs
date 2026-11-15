using Data;
using DataLayer;
using Domain.DTOs.Center;

namespace Bussiness
{
    using Bussiness.Interfaces;
    using Data.DLL;

    public class CenterBLL : ICenterService
    {
        private readonly CenterDLL _centerDLL;

        public int LastId => _centerID;
        public int _centerID { get; private set; }

        public CenterBLL(CenterDLL centerDLL)
        {
            _centerDLL = centerDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(DTO_CreateCenter addCenter ,int userId)
        {
            try
            {
            if (addCenter == null)
                throw new ArgumentNullException(nameof(addCenter),"Center data cannot be null.");

                var createCenter = new Center
                {
                    OwnerUserId = userId <= 0
                        ? throw new ArgumentException("Invalid owner ID.")
                        : userId,

                    CenterName = addCenter.CenterName,

                    CityId = addCenter.CityId <= 0
                        ? throw new ArgumentException("Invalid City ID.")
                        : addCenter.CityId,

                    CenterAddress = addCenter.CenterAddress,
                    CenterDescription = addCenter.CenterDescription,
                    CenterPhone = addCenter.CenterPhone,
                    CenterType = addCenter.CenterType,

                    CenterStatus = "pending",
                    IsActive = false,
                    IsApproved = false,
                    CreatedBy = userId <= 0 ? throw new ArgumentException("Invalid user ID.") : userId,

                    OpenTime = addCenter.OpenTime <= TimeOnly.MinValue
                        ? throw new ArgumentException("Invalid open time.")
                        : addCenter.OpenTime,

                    CloseTime = addCenter.CloseTime <= TimeOnly.MinValue
                        ? throw new ArgumentException("Invalid close time.")
                        : addCenter.CloseTime,

                    CreatedAt = DateTime.Now
                };

            int centerID = await _centerDLL.Add(createCenter);
            _centerID = centerID;
            }
            catch(Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While Creating Center", ex);
            }
            return _centerID > 0;
        }
        public async Task<bool> Update(DTO_UpdateCenter updateCenter, int centerId, int ownerId)
        {
            if (updateCenter == null)
                throw new ArgumentNullException(nameof(updateCenter),"Center data cannot be null.");

            var center = await _centerDLL.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.OwnerUserId != ownerId)
                throw new UnauthorizedAccessException("You are not the owner of this center.");

            await _centerDLL.Update(updateCenter, centerId, ownerId);

            return true;
        }
        public async Task<bool> ActiveCenter(int centerId) 
        {
            var center = await _centerDLL.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.IsActive == true)
                throw new ArgumentException("center already active");

            return await _centerDLL.ActiveCenter(centerId);
        }
        public async Task<bool> DeactiveCenter(int centerId)
        {
            var center = await _centerDLL.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.IsActive == false)
                throw new ArgumentException("Can not deactive a deactived center");

            return await _centerDLL.DeactiveCenter(centerId);
        }
        public async Task<bool> ToggleCenterStatus(int centerId, string status, int ownerId) => await _centerDLL.ToggleCenterStatus(centerId, status, ownerId);
        public async Task<bool> Delete(int centerId) => await _centerDLL.Delete(centerId);
        public async Task<List<DTO_HomePageCentersDetails>> GetAll() => await _centerDLL.GetAll();
        public async Task<List<DTO_HomePageCentersDetails>> GetAllForAdmin() => await _centerDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Center?> GetByID(int centerId)
        {
            if (centerId <= 0)
                throw new ArgumentException("Invalid center ID.");

            return await _centerDLL.GetByID(centerId);
        }
        public async Task<DTO_CenterDetails?> GetCenterDetailsById(int centerId)
        {
            if (centerId <= 0)
                throw new ArgumentException("Invalid center ID.");

            return await _centerDLL.GetCenterDetailsById(centerId);
        }
        public async Task<DTO_CenterOverview?> GetCenterOverView(int centerId)
        {
            if (centerId <= 0)
                throw new ArgumentException("Invalid center ID.");

            return await _centerDLL.GetCenterOverView(centerId);
        }
        public async Task<List<DTO_CentersListDetails>> GetAllCentersByOwnerId(int ownerId)
        {
            if (ownerId <= 0)
                throw new ArgumentException("Invalid owner ID.");

            return await _centerDLL.GetAllCentersByOwnerId(ownerId);
        }
        public async Task<List<string>> GetCenterNames() => await _centerDLL.GetCenterNames();
        // ================ Read By ================
    }
}