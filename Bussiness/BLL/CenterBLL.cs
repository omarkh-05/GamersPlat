using Data;
using DataLayer;
using Domain.DTOs.Center;
using System.Text.RegularExpressions;

namespace Bussiness
{
    using Bussiness.Interfaces;
    using Data.DLL;

    public class CenterBLL : ICenterService
    {
        private readonly CenterDLL _centerDLL;
        private readonly IAuditLogService _auditLogService;
        private readonly INotificationService _notificationService;

        public int LastId => _centerID;
        public int _centerID { get; private set; }

        public CenterBLL(CenterDLL centerDLL, IAuditLogService auditLogService, INotificationService notificationService)
        {
            _centerDLL = centerDLL;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
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
                    Slug = await GenerateUniqueSlug(addCenter.CenterName, addCenter.CityId),

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
            if (centerID > 0)
            {
                var createdCenter = await _centerDLL.GetByID(centerID);
                await _auditLogService.LogAsync(userId, "CenterCreated", "Center", centerID, null, CenterSnapshot(createdCenter));
                    await _notificationService.NotifyCenterCreatedAsync(centerID);
            }
            }
            catch(Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While Creating Center", ex);
            }
            return _centerID > 0;
        }

        private async Task<string> GenerateUniqueSlug(string centerName, short cityId)
        {
            var slug = NormalizeSlug(centerName);

            if (string.IsNullOrEmpty(slug))
                slug = "center";

            if (await _centerDLL.CenterNameExists(centerName))
            {
                var citySlug = NormalizeSlug(await _centerDLL.GetCityName(cityId) ?? string.Empty);
                if (!string.IsNullOrEmpty(citySlug))
                    slug = $"{slug}-{citySlug}";
            }

            var uniqueSlug = slug;
            var suffix = 2;
            while (await _centerDLL.SlugExists(uniqueSlug))
                uniqueSlug = $"{slug}-{suffix++}";

            return uniqueSlug;
        }

        private static string NormalizeSlug(string value)
        {
            var slug = Regex.Replace(value.ToLowerInvariant(), @"\s+", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9-]", "");
            return Regex.Replace(slug, @"-+", "-").Trim('-');
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

            var activated = await _centerDLL.ActiveCenter(centerId);
            if (activated)
                await _notificationService.NotifyCenterStatusAsync(centerId, "activated");
            return activated;
        }
        public async Task<bool> DeactiveCenter(int centerId)
        {
            var center = await _centerDLL.GetByID(centerId);
            if (center == null)
                throw new KeyNotFoundException("Center not found.");

            if (center.IsActive == false)
                throw new ArgumentException("Can not deactive a deactived center");

            var deactivated = await _centerDLL.DeactiveCenter(centerId);
            if (deactivated)
                await _notificationService.NotifyCenterStatusAsync(centerId, "deactivated");
            return deactivated;
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
        public async Task<DTO_CenterDetails?> GetCenterDetailsBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Invalid center slug.", nameof(slug));

            return await _centerDLL.GetCenterDetailsBySlug(slug);
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

        private static object? CenterSnapshot(Center? center)
            => center == null ? null : new
            {
                center.CenterName,
                center.CityId,
                center.CenterAddress,
                center.CenterDescription,
                center.CenterPhone,
                center.CenterType,
                center.CenterStatus,
                center.IsApproved,
                center.IsActive,
                center.OpenTime,
                center.CloseTime
            };
    }
}