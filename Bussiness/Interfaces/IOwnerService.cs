using Domain.DTOs.Booking;
using Domain.DTOs.Center;
using Domain.DTOs.Owner;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Domain.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IOwnerService
    {
        Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerUserId, DateOnly? from = null, DateOnly? to = null);
        Task<DTO_UserInfoRequest?> GetOwnerInfo(int ownerUserId);
        Task<bool?> UpdateOwnerInfo(int ownerUserId, DTO_UpdateUserInfoRequest request);
        Task<bool> ChangeOwnerPassword(int ownerUserId, Domain.DTOs.Auth.ChangePasswordRequest request);
        Task<List<DTO_CentersListDetails>> GetAllCentersForOwner(int ownerUserId);
        Task<DTO_CenterDetails> GetOwnerCenterDetails(int centerId, int ownerUserId);
        Task<DTO_CenterDetails> CreateOwnerCenter(DTO_CreateCenter request, int ownerUserId);
        Task<DTO_CenterDetails> UpdateOwnerCenter(int centerId, DTO_UpdateCenter request, int ownerUserId);
        Task<bool> DeleteOwnerCenter(int centerId, int ownerUserId);
        Task<DTO_OwnerWorkspace> GetOwnerWorkspace(int centerId, int ownerUserId);
        Task<List<DTO_OwnerResource>> GetOwnerResources(int centerId, int ownerUserId);
        Task<DTO_OwnerResource> CreateOwnerResource(int centerId, DTO_AddResource request, int ownerUserId);
        Task<DTO_OwnerResource> UpdateOwnerResource(int centerId, int resourceId, DTO_UpdateResource request, int ownerUserId);
        Task<bool> DeleteOwnerResource(int centerId, int resourceId, int ownerUserId);
        Task<DTO_OwnerResource> UpdateOwnerResourceUnits(int centerId, int resourceId, DTO_OwnerResourceUnitsUpdate request, int ownerUserId);
        Task<List<DTO_OwnerService>> GetOwnerServices(int centerId, int ownerUserId);
        Task<DTO_OwnerService> CreateOwnerService(int centerId, DTO_AddService request, int ownerUserId);
        Task<DTO_OwnerService> UpdateOwnerService(int centerId, int serviceId, DTO_UpdateService request, int ownerUserId);
        Task<bool> DeleteOwnerService(int centerId, int serviceId, int ownerUserId);
        Task<List<DTO_BookingDetails>> GetOwnerBookings(int ownerUserId, DTO_OwnerBookingsQuery query);
        Task<DTO_BookingDetails> GetOwnerBooking(int bookingId, int ownerUserId);
        Task<DTO_BookingDetails> UpdateOwnerBookingStatus(int bookingId, DTO_OwnerBookingStatusUpdate request, int ownerUserId);
        Task<List<DTO_OwnerCustomerRequest>> GetOwnerRequests(int ownerUserId, string? type, string? status);
        Task<DTO_OwnerCustomerRequest> GetOwnerRequest(int requestId, int ownerUserId);
        Task<DTO_OwnerCustomerRequest> UpdateOwnerRequestStatus(int requestId, DTO_OwnerRequestStatusUpdate request, int ownerUserId);
        Task<List<DTO_OwnerEmployee>> GetOwnerEmployees(int ownerUserId, int? centerId);
        Task<DTO_OwnerEmployee> CreateOwnerEmployee(DTO_OwnerEmployeeCreateRequest request, int ownerUserId);
        Task<DTO_OwnerEmployee> UpdateOwnerEmployee(int employeeId, DTO_OwnerEmployeeUpdateRequest request, int ownerUserId);
        Task<bool> DeleteOwnerEmployee(int employeeId, int ownerUserId);
        Task<bool> AcceptBooking(int bookingId, int ownerUserId);
        Task<bool> RejectBooking(int bookingId, int ownerUserId);
        Task<List<DTO_BookingDetails>> GetAllBookings(int ownerUserId);
        Task<DTO_BookingDetails> ViewBookingDetails(int bookingId, int ownerUserId);
    }
}