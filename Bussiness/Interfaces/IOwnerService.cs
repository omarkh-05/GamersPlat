using Domain.DTOs.Booking;
using Domain.DTOs.Owner;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IOwnerService
    {
        Task<bool> AcceptBooking(int bookingId, int ownerUserId);
        Task<bool> RejectBooking(int bookingId, int ownerUserId);
        Task<List<DTO_BookingDetails>> GetAllBookings(int ownerUserId);
        Task<DTO_BookingDetails> ViewBookingDetails(int bookingId, int ownerUserId);
    }
}