using Bussiness.Helpers;
using Bussiness.Interfaces;
using Bussiness;
using Data;
using Data.DLL;
using DataLayer;
using Domain.DTOs.Admin;
using Domain.DTOs.Center;
using Domain.DTOs.User;

namespace Bussiness.BLL
{
    public class AdminBLL
    {
        private readonly IUser _user;
        private readonly CenterBLL _center;
        private readonly PlayerBLL _player;
        private readonly OwnerBLL _owner;
        private readonly ServiceBLL _service;
        private readonly DeviceBLL _device;
        private readonly ResourcesTypeBLL _resource;
        private readonly CityBLL _city;
        private readonly CountryBLL _country;
        public AdminBLL(IUser user, CenterBLL center, PlayerBLL player, OwnerBLL owner, ServiceBLL service, CityBLL city, CountryBLL country, DeviceBLL device, ResourcesTypeBLL resource)
        {
            _user = user;
            _center = center;
            _player = player;
            _owner = owner;
            _service = service;
            _city = city;
            _country = country;
            _device = device;
            _resource = resource;
        }

        // ================ Admin Analytics ================
        public async Task<DTO_AdminStatistics> GetSystemStatistics(DateTime? from = null, DateTime? to = null)
        {
            if (from.HasValue && to.HasValue && from > to) throw new ArgumentException("From date cannot be later than To date.");
            return await AdminDLL.GetSystemStatistics(from, to);
        }
        // ================ Admin Analytics ================


        // ================ Center Managament ================
        public async Task<List<DTO_HomePageCentersDetails>> GetAll() => await _center.GetAll();
        public async Task<bool> ApproveCenter(int centerId)
        {
            if (centerId <= 0) throw new ArgumentException("Invalid center ID.");
            return await AdminDLL.ApproveCenter(centerId);
        }
        public async Task<bool> RejectCenter(int centerId, string reason)
        {
            if (centerId <= 0) throw new ArgumentException("Invalid center ID.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Rejection reason is required.");
            return await AdminDLL.RejectCenter(centerId, reason);
        }
        // ================ Center Managament ================


        // ================ User Management ================
        public async Task<List<DTO_UserListResponse>> GetUsers() => await _user.GetAll();
        public async Task<bool> BlockUser(int userId, string? reason = null)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.");
            var user = await _user.GetById(userId);
            if (user == null) throw new Exception("User not found.");
            user.IsActive = false;
            // Log reason via event log helper
            if (!string.IsNullOrWhiteSpace(reason)) EventLog_Helper.WriteEventLog($"Admin blocked user {userId}", new Exception(reason));
            return await _user.Update(userId, user);
        }
        public async Task<bool> UnblockUser(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.");
            var user = await _user.GetById(userId);
            if (user == null) throw new Exception("User not found.");
            user.IsActive = true;
            return await _user.Update(userId, user);
        }
        // ================ User Management ================


        // ================ City / Country Management ================
        public async Task<bool> AddCity(City city) => await _city.Add(city);
        public async Task<bool> UpdateCity(City city) => await _city.Update(city);
        public async Task<bool> DeleteCity(int cityId) => await _city.Delete(cityId);

        public async Task<bool> AddCountry(Country country) => await _country.Add(country);
        public async Task<bool> UpdateCountry(Country country) => await _country.Update(country);
        public async Task<bool> DeleteCountry(int countryId) => await _country.Delete(countryId); 
        // ================ City / Country Management ================


        // ================ Device / Resource Types ================
        public async Task<bool> AddDevice(Device device) => await _device.Add(device);
        public async Task<bool> UpdateDevice(Device device) => await _device.Update(device);
        public async Task<bool> DeleteDevice(int id) => await _device.Delete(id);

        public async Task<bool> AddResourcesType(ResourcesType rt) => await _resource.Add(rt);
        public async Task<bool> UpdateResourcesType(ResourcesType rt) => await _resource.Update(rt);
        public async Task<bool> ToggleResourcesTypeActive(int centerId, int resourceId) => await _resource.UpdateActiveStatus(centerId, resourceId);
        public async Task<bool> DeleteResourcesType(int id) => await _resource.Delete(id);
        // ================ Device / Resource Types ================


        // ================ Services Management ================
        public async Task<bool> AddService(Service service) => await _service.Add(service);
        public async Task<bool> UpdateService(Service service) => await _service.Update(service);
        public async Task<bool> ToggleServiceActive(int serviceId, int centerId) => await _service.UpdateActiveStatus(serviceId, centerId);
        public async Task<bool> DeleteService(int serviceId, int centerId) => await _service.Delete(serviceId, centerId);
        public async Task<List<Service>> GetAllServices() => await _service.GetAll();
        public async Task<Service?> GetServiceById(int id) => await _service.GetByID(id);
        // ================ Services Management ================

        /*
        // ================ Tournament Moderation ================
        public async Task<List<Tournament>> GetAllTournaments() => await AdminDLL.GetAllTournaments();
        public async Task<Tournament?> GetTournamentById(int id) => await AdminDLL.GetTournamentById(id);
        public async Task<bool> DeleteTournament(int id) => await AdminDLL.DeleteTournament(id);
        public async Task<bool> ApproveTournament(int id) => await AdminDLL.ApproveTournamentNotification(id);
        // ================ Tournament Moderation ================


        // ================ Offer Management ================
        public async Task<List<Offer>> GetAllOffers() => await AdminDLL.GetAllOffers();
        public async Task<Offer?> GetOfferById(int id) => await AdminDLL.GetOfferById(id);
        public async Task<bool> DeleteOffer(int id) => await AdminDLL.DeleteOffer(id);
        public async Task<bool> ToggleOfferActive(int offerId, bool isActive) => await AdminDLL.ToggleOfferActive(offerId, isActive);
        // ================ Offer Management ================


        // ================ Rating Moderation ================
        public async Task<List<Review>> GetAllReviews() => await AdminDLL.GetAllReviews();
        public async Task<Review?> GetReviewById(int id) => await AdminDLL.GetReviewById(id);
        public async Task<bool> DeleteReview(int id) => await AdminDLL.DeleteReview(id);
        // ================ Rating Moderation ================

        
        // ================ Announcements ================
        public async Task<bool> SendAnnouncement(string title, string message, string audience) => await AdminDLL.SendAnnouncement(title, message, audience);
        // ================ Announcements ================


        // ================ Reported Content Management ================
        public async Task<bool> SubmitReport(int reporterUserId, string reportType, int? referenceId, string reason, int? centerId = null) => await AdminDLL.SubmitReport(reporterUserId, reportType, referenceId, reason, centerId);
        public async Task<List<Notification>> GetReports() => await AdminDLL.GetReports();
        public async Task<bool> ResolveReport(int notificationId, string action, int? relatedId = null) => await AdminDLL.ResolveReport(notificationId, action, relatedId);
        // ================ Reported Content Management ================
        */
    }
}
