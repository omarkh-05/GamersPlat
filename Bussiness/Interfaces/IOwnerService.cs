using Domain.DTOs.Auth;
using Domain.DTOs.Booking;
using Domain.DTOs.Center;
using Domain.DTOs.Owner;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Domain.DTOs.Staff;
using Domain.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IOwnerService
    {
        Task<DTO_UserInfoRequest?> GetOwnerInfo(int ownerId);
        Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerId);
        Task<bool?> UpdateOwnerInfo(int ownerId, DTO_UpdateUserInfoRequest updateUserInfoRequest);
        Task<bool?> ChangeOwnerPassword(int ownerId, ChangePasswordRequest changePasswordRequest);

        Task<bool> AddCenter(DTO_CreateCenter addCenter);
        Task<bool> UpdateCenter(DTO_UpdateCenter updateCenter, int ownerUserId);
        Task<bool> ActiveCenter(int centerId, int ownerUserId);
        Task<bool> DeactiveCenter(int centerId, int ownerUserId);
        Task<bool> ToggleCenterStatus(int centerId, int ownerUserId, string status);
        Task<List<DTO_CentersListDetails>> GetAllCentersForOwner(int ownerId);
        Task<DTO_CenterOverview?> GetCenterOverView(int centerId);

        Task<bool> AddResource(DTO_AddResource resource, int ownerUserId);
        Task<bool> UpdateResource(DTO_UpdateResource resource, int ownerUserId);
        Task<bool> ActiveResource(int resourceId, int centerId, int ownerUserId);
        Task<bool> DeactiveResource(int resourceId, int centerId, int ownerUserId);
        Task<List<DTO_ResourceInfo>> GetAllResourceByCenterId(int centerId, int ownerUserId);

        Task<bool> AddService(DTO_AddService service, int ownerUserId);
        Task<bool> UpdateService(DTO_UpdateService service, int ownerUserId);
        Task<bool> ActiveService(int serviceId, int centerId, int ownerUserId);
        Task<bool> DeactiveService(int serviceId, int centerId, int ownerUserId);
        Task<List<DTO_ServicesInfo>> GetAllServicesByCenterId(int centerId, int ownerUserId);

        Task<bool> AddBooking(DTO_BookingBase booking, int ownerUserId);
        Task<bool> UpdateBooking(DTO_UpdateBooking booking);
        Task<bool> AcceptBooking(int bookingId, int ownerUserId);
        Task<bool> RejectBooking(int bookingId, int ownerUserId);
        Task<DTO_BookingDetails> ViewBookingDetails(int bookingId, int ownerUserId);
        Task<List<DTO_BookingDetails>> GetAllBookings(int ownerUserId);
        Task<List<DTO_BookingDetails>> GetAllUBookings_ByCenterId(int centerId, int ownerUserId);

        Task<bool> AddStaffMember(DTO_AddStaffMember staffMember);
        Task<bool> UpdateStaffMember(int userId, DTO_UpdateStaffMember request);
        Task<bool> DeleteStaffMember(int userId);
        Task<bool> AddStaffRoleByCenterId(DTO_StaffRole role);
        Task<bool> SetRoleToMember(DTO_SetRoleToMember request);
        Task<List<DTO_StaffDetails>> GetAllStaffByOwnerId(int ownerId);
        Task<List<DTO_StaffDetails>> GetAllStaffByCenterId(int centerId);
    }
}