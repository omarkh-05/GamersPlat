using Domain.DTOs.Auth;
using Domain.DTOs.Booking;
using Domain.DTOs.Center;
using Domain.DTOs.Invoice;
using Domain.DTOs.Owner;
using Domain.DTOs.Resource;
using Domain.DTOs.Service;
using Domain.DTOs.Staff;
using Domain.DTOs.User;
using Domain.DTOs.Pagination;

namespace Bussiness.Interfaces
{
    public interface IOwnerService
    {
        Task<DTO_UserInfoRequest?> GetOwnerInfo(int ownerId);
        Task<DTO_OwnerDashboard?> GetOwnerDashboard(int ownerId);
        Task<bool?> UpdateOwnerInfo(int ownerId, DTO_UpdateUserInfoRequest updateUserInfoRequest);
        Task<bool?> ChangeOwnerPassword(int ownerId, ChangePasswordRequest changePasswordRequest);

        Task<bool> AddCenter(DTO_CreateCenter addCenter, int ownerId);
        Task<bool> UpdateCenter(DTO_UpdateCenter updateCenter, int centerId ,int ownerId);
        Task<bool> ActiveCenter(int centerId, int ownerUserId);
        Task<bool> DeactiveCenter(int centerId, int ownerUserId);
        Task<bool> ToggleCenterStatus(int centerId, int ownerUserId, string status);
        Task<List<DTO_CentersListDetails>> GetAllCentersForOwner(int ownerId);
        Task<PaginatedResponse<DTO_CentersListDetails>> GetAllCentersForOwner(int ownerId, PaginationParameters pagination);
        Task<DTO_CenterOverview?> GetCenterOverView(int centerId);

        Task<List<DTO_ResourceInfo>> GetAllResourceByCenterId(int centerId, int ownerUserId);
        Task<bool> AddResourceToCenter(DTO_AddResource resource, int centerId, int ownerUserId);
        Task<bool> UpdateCenterResource(DTO_UpdateResource resource, int centerId, int centerResourceId, int ownerUserId);
        Task<bool> ActiveCenterResource(int resourceId, int centerId, int ownerUserId);
        Task<bool> DeactiveCenterResource(int resourceId, int centerId, int ownerUserId);

        Task<List<DTO_ServicesInfo>> GetCenterServices(int centerId, int ownerUserId);
        Task<bool> AddCenterService(DTO_AddCenterService service, int centerId, int ownerUserId);
        Task<bool> UpdateCenterService(DTO_UpdateCenterService service, int centerId, int centerServiceId, int ownerUserId);
        Task<bool> ActiveCenterService(int serviceId, int centerId, int ownerUserId);
        Task<bool> DeactiveCenterService(int serviceId, int centerId, int ownerUserId);

        Task<bool> AddBooking(DTO_BookingBase booking, int ownerUserId);
        Task<bool> UpdateBooking(DTO_UpdateBooking booking);
        Task<bool> AcceptBooking(int bookingId, int ownerUserId);
        Task<bool> RejectBooking(int bookingId, int ownerUserId);
        Task<bool> StartBooking(int bookingId, int ownerUserId);
        Task<DTO_InvoicePreview> GetInvoicePreview(int bookingId, int ownerUserId);
        Task<DTO_Invoice> StopBooking(int bookingId, int ownerUserId, DTO_EndBookingInvoice endBooking);
        Task<DTO_BookingDetails> ViewBookingDetails(int bookingId, int ownerUserId);
        Task<List<DTO_BookingDetails>> GetAllBookings(int ownerUserId);
        Task<PaginatedResponse<DTO_BookingDetails>> GetAllBookings(int ownerUserId, PaginationParameters pagination);
        Task<List<DTO_BookingDetails>> GetAllBookings_ByCenterId(int centerId, int ownerUserId);
        Task<PaginatedResponse<DTO_BookingDetails>> GetAllBookings_ByCenterId(int centerId, int ownerUserId, PaginationParameters pagination);

        Task<PaginatedResponse<DTO_CenterInvoices>> GetAllCenterInvoices(int centerId, int ownerId, PaginationParameters pagination);

        Task<bool> AddStaffMember(DTO_AddStaffMember staffMember);
        Task<bool> UpdateStaffMember(DTO_UpdateStaffMember request);
        Task<bool> DeleteStaffMember(int userId);
        Task<List<DTO_StaffDetails>> GetAllStaffByOwnerId(int ownerId);
        Task<List<DTO_StaffDetails>> GetAllStaffByCenterId(int centerId);
    }
}