using Data;
using Domain.DTOs.Auth;
using Domain.DTOs.Booking;
using Domain.DTOs.Invoice;
using Domain.DTOs.Player;
using Domain.DTOs.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public enum GuestBookingFailure
    {
        None,
        ExistingAccount,
        RegistrationFailed,
        BookingFailed
    }

    public readonly record struct GuestBookingResult(bool Success, int BookingId = 0, string? Message = null, GuestBookingFailure Failure = GuestBookingFailure.None, TokenResponse? Token = null);

    public interface IBookingService
    {
        Task<bool> Add(DTO_BookingBase dto, int userId,string? customerName, string? phone);
        Task<GuestBookingResult> CreateGuestBookingAsync(DTO_AddGuestBooking dto);
        Task<bool> Update(DTO_UpdateBooking updateBooking, int userId);
        Task<bool> Delete(int bookingID);
        Task<bool> Cancel(DTO_CancelBooking cancelBooking, int userId);
        Task<List<DTO_BookingDetails>> GetAll();
        Task<PaginatedResponse<DTO_BookingDetails>> GetAll(PaginationParameters pagination);
        Task<List<DTO_PlayerBookingsInfo>> GetByPlayerId(int userId);
        Task<PaginatedResponse<DTO_PlayerBookingsInfo>> GetByPlayerId(int userId, PaginationParameters pagination);
        Task<Booking?> GetByID(int bookingID);
        Task<bool> AcceptBooking(int bookingId, int userId);
        Task<bool> RejectBooking(int bookingId, int userId);
        Task<bool> StartBooking(int bookingId, int userId);
        Task<DTO_InvoicePreview?> GetInvoicePreview(int bookingId);
        Task<DTO_Invoice?> StopBooking(int bookingId, int userId, DTO_EndBookingInvoice endBooking);
        Task<List<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId);
        Task<PaginatedResponse<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId, PaginationParameters pagination);
        Task<List<DTO_BookingDetails>> GetBookingsByCenterId(int centerId);
        Task<PaginatedResponse<DTO_BookingDetails>> GetBookingsByCenterId(int centerId, PaginationParameters pagination);
        Task<List<DTO_StartEndTime>?> GetFullBookedHours(int centerId, int centerResourceId, DateOnly date);
        // helper to compute booked quantity for availability check // no-op
        string? LastError { get; }
        // other booking operations can be added as needed
        int LastId { get; }
    }
}
