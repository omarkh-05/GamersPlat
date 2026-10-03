using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Booking;
using Domain.DTOs.Player;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class BookingDLL
    {
        private readonly GamersPlatDbContext _db;

        public BookingDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(Booking booking)
        {
            try
            {
                _db.Bookings.Add(booking);
                await _db.SaveChangesAsync();

                return booking.BookingId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add Booking Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(Booking booking)
        {
            try
            {
                var existing = await _db.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

                if (existing == null)
                    return false;

                _db.Entry(existing).CurrentValues.SetValues(booking);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Booking Error", ex);
                return false;
            }
        }

        public async Task<bool> Delete(int bookingId)
        {
            try
            {
                var existing = await _db.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (existing == null)
                    return false;

                _db.Bookings.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete Booking Error", ex);
                return false;
            }
        }

        public async Task<bool> Cancel(int bookingId)
        {
            try
            {
                var existing = await _db.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (existing == null)
                    return false;

                existing.Status = "Cancelled";

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Cancel Booking Error", ex);
                return false;
            }
        }

        public async Task<List<Booking>> GetAll()
        {
            try
            {
                return await _db.Bookings
                    .Include(b => b.Center)
                    .Include(b => b.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Bookings Error", ex);
                return new List<Booking>();
            }
        }

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<Booking?> GetByID(int bookingId)
        {
            try
            {
                return await _db.Bookings
                    .Include(b => b.Center)
                    .Include(b => b.Offer)
                    .Include(b => b.ResourcesType)
                    .Include(b => b.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Booking By ID Error", ex);
                return null;
            }
        }

        public async Task<List<DTO_PlayerBookingsInfo>> GetByUserId(int userId)
        {
            try
            {
                return await _db.Bookings
                    .Where(b => b.UserId == userId)
                    .AsNoTracking()
                    .Select(b => new DTO_PlayerBookingsInfo
                    {
                        CenterName = b.Center.CenterName,
                        Resource = b.ResourcesType.Device.DeviceName,
                        OfferId = b.OfferId,
                        Quantity = b.Quantity,
                        TotalPrice = b.TotalPrice,
                        EarnedPoints = b.EarnedPoints,
                        Status = b.Status,
                        BookingDate = b.BookingDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Bookings By UserID Error", ex);
                return new List<DTO_PlayerBookingsInfo>();
            }
        }

        public async Task<int> GetBookedQuantity(int resourcesTypeId, DateOnly date)
        {
            try
            {
                return await _db.Bookings
                    .Where(b =>
                        b.ResourcesTypeId == resourcesTypeId &&
                        b.BookingDate == date &&
                        b.Status != "Cancelled")
                    .SumAsync(b => (int?)b.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Booked Quantity Error", ex);
                return 0;
            }
        }

        public async Task<int> GetBookedQuantityForTimeSlot(
            int resourcesTypeId,
            DateOnly date,
            TimeOnly startTime,
            TimeOnly? endTime)
        {
            try
            {
                var reqEnd = endTime ?? startTime.AddHours(1);

                return await _db.Bookings
                    .Where(b =>
                        b.ResourcesTypeId == resourcesTypeId &&
                        b.BookingDate == date &&
                        b.Status != "Cancelled" &&
                        (b.StartTime < reqEnd &&
                         (b.EndTime ?? b.StartTime.AddHours(1)) > startTime))
                    .SumAsync(b => (int?)b.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Booked Quantity For Time Slot Error",
                    ex);

                return 0;
            }
        }

        public async Task<int> GetBookedQuantityForTimeSlotExcludingBooking(
            int resourcesTypeId,
            DateOnly date,
            TimeOnly startTime,
            TimeOnly? endTime,
            int excludeBookingId)
        {
            try
            {
                var reqEnd = endTime ?? startTime.AddHours(1);

                return await _db.Bookings
                    .Where(b =>
                        b.ResourcesTypeId == resourcesTypeId &&
                        b.BookingDate == date &&
                        b.Status != "Cancelled" &&
                        b.BookingId != excludeBookingId &&
                        (b.StartTime < reqEnd &&
                         (b.EndTime ?? b.StartTime.AddHours(1)) > startTime))
                    .SumAsync(b => (int?)b.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Booked Quantity For Time Slot Excluding Booking Error",
                    ex);

                return 0;
            }
        }

        // ================ Read By ================


        // ================ Owner Booking Management ================
        public async Task<bool> Accept_RejectBooking(int bookingId, string status)
        {
            try
            {
                var booking = await _db.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                    return false;

                booking.Status = status;
                booking.UpdatedAt = DateTime.UtcNow;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Update Booking Status Error",
                    ex);

                return false;
            }
        }

        public async Task<List<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId)
        {
            try
            {
                return await _db.Bookings
                    .Where(b => b.Center.OwnerUserId == ownerId)
                    .AsNoTracking()
                    .OrderBy(b => b.BookingDate)
                    .Select(b => new DTO_BookingDetails
                    {
                        CustomerName = b.CustomerName,
                        PhoneNumber = b.PhoneNumber,
                        CenterName = b.Center.CenterName,
                        ResourcesType =
                            b.ResourcesType.Device.DeviceName +
                            " - " +
                            b.ResourcesType.RoomType,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime.HasValue
                            ? b.EndTime.Value.ToString("HH:mm")
                            : "Open Time",
                        Duration = b.EndTime.HasValue
                            ? $"{(b.EndTime.Value - b.StartTime).TotalMinutes} Minutes"
                            : "Open Time",
                        BookingDate = b.BookingDate,
                        TotalPrice = b.TotalPrice,
                        Status = b.Status,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Bookings By Owner Error",
                    ex);

                return new List<DTO_BookingDetails>();
            }
        }

        public async Task<List<DTO_BookingDetails>> GetBookingsByCenterId(int centerId)
        {
            try
            {
                return await _db.Bookings
                    .Where(b => b.CenterId == centerId)
                    .AsNoTracking()
                    .OrderBy(b => b.BookingDate)
                    .Select(b => new DTO_BookingDetails
                    {
                        CustomerName = b.CustomerName,
                        PhoneNumber = b.PhoneNumber,
                        CenterName = b.Center.CenterName,
                        ResourcesType =
                            b.ResourcesType.Device.DeviceName +
                            " - " +
                            b.ResourcesType.RoomType,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime.HasValue
                            ? b.EndTime.Value.ToString("HH:mm")
                            : "Open Time",
                        Duration = b.EndTime.HasValue
                            ? $"{(b.EndTime.Value - b.StartTime).TotalMinutes} Minutes"
                            : "Open Time",
                        BookingDate = b.BookingDate,
                        TotalPrice = b.TotalPrice,
                        Status = b.Status,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get Bookings By Center Error",
                    ex);

                return new List<DTO_BookingDetails>();
            }
        }

        // ================ Owner Booking Management ================
    }
}