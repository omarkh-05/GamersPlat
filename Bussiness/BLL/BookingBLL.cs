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

        public bool Add()
        {
            _bookingID = BookingDLL.Add(_booking);
            return _bookingID > 0;
        }

        public bool Update() => BookingDLL.Update(_booking);

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
