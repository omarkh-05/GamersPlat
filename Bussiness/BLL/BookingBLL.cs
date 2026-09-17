using Data;
using DataLayer;
using Domain.DTOs.Booking;
using Domain.DTOs.Player;
namespace Bussiness
{
    public class BookingBLL
    {
        public int _bookingID { get; private set; }
        private readonly ResourcesTypeBLL _resourcesTypeBLL;
        public BookingBLL(ResourcesTypeBLL resourcesTypeBLL)
        {
            _resourcesTypeBLL = resourcesTypeBLL;
        }
        public string? LastError { get; private set; }

        // ================ CRUD ================
        public async Task<bool> Add(DTO_AddBooking addBooking)
        {
            try
            {
                var rt = await _resourcesTypeBLL.GetByID(addBooking.ResourcesTypeId);
                if (rt == null)
                {
                    LastError = "Resource type not found";
                    return false;
                }

                var booked = await BookingDLL.GetBookedQuantity(addBooking.ResourcesTypeId, addBooking.BookingDate);
                var booking = new Booking
                {
                    UserId = addBooking.UserId,
                    CenterId = addBooking.CenterId,
                    ResourcesTypeId = addBooking.ResourcesTypeId,
                    BookingDate = addBooking.BookingDate,
                    StartTime = addBooking.StartTime,
                    EndTime = addBooking.EndTime,
                    CustomerName = addBooking.CustomerName,
                    PhoneNumber = addBooking.PhoneNumber,
                    OfferId = addBooking.OfferId
                };
                int bookingID = await BookingDLL.Add(booking);
                _bookingID = bookingID;
                return bookingID > 0;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }
        public async Task<bool> Update(DTO_UpdateBooking updaetBooking)
        {
            // Validate availability when changing booking date/resource/quantity
            try
            {
                var existingTask = await BookingDLL.GetByID(updaetBooking.BookingId);
                var existing = existingTask;

                if (existing == null)
                {
                    LastError = "Booking not found";
                    return false;
                }

                var booking = new Booking
                {
                    BookingId = updaetBooking.BookingId,
                    BookingDate = updaetBooking.BookingDate,
                    StartTime = updaetBooking.StartTime,
                    EndTime = updaetBooking.EndTime,
                    CustomerName = updaetBooking.CustomerName,
                    PhoneNumber = updaetBooking.PhoneNumber
                };

                return await BookingDLL.Update(booking);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }
        public async Task<bool> Delete(int bookingID) => await BookingDLL.Delete(bookingID);
        public async Task<bool> Cancel(int bookingID) => await BookingDLL.Cancel(bookingID);
        public async Task<List<Booking>> GetAll() => await BookingDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<DTO_PlayerBookingsInfo>> GetByUserId(int userId) => await BookingDLL.GetByUserId(userId);
        public async Task<Booking?> GetByID(int bookingID) => await BookingDLL.GetByID(bookingID);
        // ================ Read By ================


        // ================ Owner Booking Managament ================
        public async Task<bool> Accept_RejectBooking(int bookingId,string status) => await BookingDLL.Accept_RejectBooking(bookingId,status);
        public async Task<List<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId) => await BookingDLL.GetBookingsByOwnerId(ownerId);
        public async Task<List<DTO_BookingDetails>> GetBookingsByCenterId(int centerId) => await BookingDLL.GetBookingsByCenterId(centerId);
        // ================ Owner Booking Managament ================
    }
}
