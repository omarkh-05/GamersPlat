using Data;
using Domain.DTOs.Booking;
using Domain.DTOs.Player;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IBookingService
    {
        Task<bool> Add(DTO_AddBooking addBooking);
        Task<bool> Update(DTO_UpdateBooking updateBooking);
        Task<bool> Delete(int bookingID);
        Task<bool> Cancel(int bookingID);
        Task<List<Booking>> GetAll();
        Task<List<DTO_PlayerBookingsInfo>> GetByPlayerId(int userId);
        Task<Booking?> GetByID(int bookingID);
        Task<bool> Accept_RejectBooking(int bookingId, string status);
        Task<List<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId);
        Task<List<DTO_BookingDetails>> GetBookingsByCenterId(int centerId);
        // helper to compute booked quantity for availability check // no-op
        Task<int> GetBookedQuantity(int centerId, int resourcesTypeId, DateOnly date);
        string? LastError { get; }
        // other booking operations can be added as needed
        int LastId { get; }
    }
}
