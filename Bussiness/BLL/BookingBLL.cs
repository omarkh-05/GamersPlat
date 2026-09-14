using Data;
using DataLayer;
using Domain.DTOs.Player;
namespace Bussiness
{
    public class BookingBLL
    {
        private readonly ResourcesTypeBLL _resourcesTypeBLL;
        public BookingBLL(ResourcesTypeBLL resourcesTypeBLL)
        {
            _resourcesTypeBLL = resourcesTypeBLL;
        }
        public string? LastError { get; private set; }

        // ================ CRUD ================
        public async Task<bool> Add(Booking booking)
        {
            try
            {
                var rt = await _resourcesTypeBLL.GetByID(booking.ResourcesTypeId);
                if (rt == null)
                {
                    LastError = "Resource type not found";
                    return false;
                }

                var booked = await BookingDLL.GetBookedQuantity(booking.ResourcesTypeId, booking.BookingDate);
                var available = rt.TotalQuantity - booked;
                if (booking.Quantity > available)
                {
                    LastError = "Not enough availability";
                    return false;
                }

               int bookingID = await BookingDLL.Add(booking);
                return bookingID > 0;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }
        public async Task<bool> Update(Booking booking)
        {
            // Validate availability when changing booking date/resource/quantity
            try
            {
                var existingTask = BookingDLL.GetByID(booking.BookingId);
                existingTask.Wait();
                var existing = existingTask.Result;

                if (existing == null)
                {
                    LastError = "Booking not found";
                    return false;
                }

                // if resource/date/quantity changed, validate availability
                bool needsCheck = existing.BookingDate != booking.BookingDate || existing.ResourcesTypeId != booking.ResourcesTypeId || existing.Quantity != booking.Quantity;
                if (needsCheck)
                {
                    var rt = await _resourcesTypeBLL.GetByID(booking.ResourcesTypeId);
                    if (rt == null)
                    {
                        LastError = "Resource type not found";
                        return false;
                    }

                    var booked = await BookingDLL.GetBookedQuantity(booking.ResourcesTypeId, booking.BookingDate);
                    // remove existing booking's quantity from booked count if same resource/date
                    if (existing.ResourcesTypeId == booking.ResourcesTypeId && existing.BookingDate == booking.BookingDate)
                    {
                        booked -= existing.Quantity;
                    }

                    var available = rt.TotalQuantity - booked;
                    if (booking.Quantity > available)
                    {
                        LastError = "Not enough availability";
                        return false;
                    }
                }

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
    }
}
