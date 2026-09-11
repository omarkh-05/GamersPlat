using Data;
using DataLayer;
namespace Bussiness
{
    public class BookingBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Booking _booking;
        public int _bookingID = -1;

        public BookingBLL()
        {
            _booking = new Booking();
            _mode = enMode.AddMode;
        }

        public BookingBLL(Booking booking)
        {
            _booking = booking;
            _mode = enMode.UpdateMode;
        }

        public Booking CurrentBooking { get => _booking; set => _booking = value; }
        public string? LastError { get; private set; }

        public bool Add()
        {
            // Validate resource availability
            try
            {
                var rt = DataLayer.ResourcesTypeDLL.GetByID(_booking.ResourcesTypeId);
                if (rt == null)
                {
                    LastError = "Resource type not found";
                    return false;
                }

                var booked = BookingDLL.GetBookedQuantity(_booking.ResourcesTypeId, _booking.BookingDate);
                var available = rt.TotalQuantity - booked;
                if (_booking.Quantity > available)
                {
                    LastError = "Not enough availability";
                    return false;
                }

                _bookingID = BookingDLL.Add(_booking);
                return _bookingID > 0;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool Update()
        {
            // Validate availability when changing booking date/resource/quantity
            try
            {
                var existingTask = BookingDLL.GetByID(_booking.BookingId);
                existingTask.Wait();
                var existing = existingTask.Result;

                if (existing == null)
                {
                    LastError = "Booking not found";
                    return false;
                }

                // if resource/date/quantity changed, validate availability
                bool needsCheck = existing.BookingDate != _booking.BookingDate || existing.ResourcesTypeId != _booking.ResourcesTypeId || existing.Quantity != _booking.Quantity;
                if (needsCheck)
                {
                    var rt = DataLayer.ResourcesTypeDLL.GetByID(_booking.ResourcesTypeId);
                    if (rt == null)
                    {
                        LastError = "Resource type not found";
                        return false;
                    }

                    var booked = BookingDLL.GetBookedQuantity(_booking.ResourcesTypeId, _booking.BookingDate);
                    // remove existing booking's quantity from booked count if same resource/date
                    if (existing.ResourcesTypeId == _booking.ResourcesTypeId && existing.BookingDate == _booking.BookingDate)
                    {
                        booked -= existing.Quantity;
                    }

                    var available = rt.TotalQuantity - booked;
                    if (_booking.Quantity > available)
                    {
                        LastError = "Not enough availability";
                        return false;
                    }
                }

                return BookingDLL.Update(_booking);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool Delete(int bookingID) => BookingDLL.Delete(bookingID);

        public static Task<Booking?> GetByID(int bookingID) => BookingDLL.GetByID(bookingID);

        public static Task<List<Booking>> GetAll() => BookingDLL.GetAll();

        public static Task<List<Booking>> GetByUserId(int userId) => BookingDLL.GetByUserId(userId);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
