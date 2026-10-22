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
        public enum BookingStatus
        {
            pending,
            accepted,
            rejected,
            cancelled ,
            completed
        }
        public enum AddBookingStatus
        {
            Success,
            SlotUnavailable,
            DuplicateBooking,
            LimitReached,
            Error
        }

        private readonly GamersPlatDbContext _db;
        public BookingDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        public readonly record struct AddBookingResult(AddBookingStatus Status, int BookingId = 0)
        {
            public bool IsSuccess => Status == AddBookingStatus.Success;
        }

        // ================ CRUD ================
        public async Task<AddBookingResult> AddIfAvailable(Booking booking, int totalQuantity, bool isStaffOrAdmin)
        {
            var tx = _db.Database.CurrentTransaction;
            var ownsTransaction = tx == null;
            if (ownsTransaction)
                tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                if (!isStaffOrAdmin)
                {
                    if (await HasDuplicateBookingInternal(booking))
                    {
                        if (ownsTransaction) await tx!.RollbackAsync();
                        return new(AddBookingStatus.DuplicateBooking);
                    }

                    if (await CountActiveBookings(booking.UserId) >= 5)
                    {
                        if (ownsTransaction) await tx!.RollbackAsync();
                        return new(AddBookingStatus.LimitReached);
                    }
                }

                var booked = await GetBookedQuantityForTimeSlot(booking.ResourcesTypeId, booking.BookingDate, booking.StartTime, booking.EndTime!.Value);

                if (booked + booking.Quantity > totalQuantity)
                {
                    if (ownsTransaction) await tx!.RollbackAsync();
                    return new(AddBookingStatus.SlotUnavailable);
                }

                _db.Bookings.Add(booking);
                await _db.SaveChangesAsync();
                if (ownsTransaction) await tx!.CommitAsync();

                return new(AddBookingStatus.Success, booking.BookingId);
            }
            catch (Exception ex)
            {
                if (ownsTransaction)
                {
                    try { await tx!.RollbackAsync(); } catch { }
                }
                EventLog_Helper.WriteEventLog("Add Booking Error", ex);
                return new(AddBookingStatus.Error);
            }
            finally
            {
                if (ownsTransaction && tx != null)
                    await tx.DisposeAsync();
            }
        }
        public async Task<bool> UpdateIfAvailable(Booking existing,int totalQuantity)
        {
            var tx = _db.Database.CurrentTransaction;
            var ownsTransaction = tx == null;

            try
            {
                if (ownsTransaction)
                {
                    tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
                }

                var booked = await GetBookedQuantityForTimeSlotExcludingBooking(existing.ResourcesTypeId, existing.BookingDate, existing.StartTime, existing.EndTime!.Value, existing.BookingId);

                if (booked + existing.Quantity > totalQuantity)
                {
                    if (ownsTransaction)
                        await tx!.RollbackAsync();

                    return false;
                }

                await _db.SaveChangesAsync();

                if (ownsTransaction)
                    await tx!.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                if (ownsTransaction && tx != null)
                        await tx.RollbackAsync();

                EventLog_Helper.WriteEventLog("Update Booking Error", ex);
                throw;
            }
            finally
            {
                if (ownsTransaction && tx != null)
                    await tx.DisposeAsync();
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
        public async Task<bool> Cancel()
        {
            try
            {
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
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Booking By ID Error", ex);
                return null;
            }
        }
        public async Task<bool> FindBookingById(int bookingId)
        {
            try
            {
                return await _db.Bookings
                    .AsNoTracking()
                    .AnyAsync(b => b.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Find Booking By ID Error", ex);
                return false;
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
                        BookingId = b.BookingId,
                        CenterName = b.Center.CenterName,
                        Resource = b.ResourcesType.Device.DeviceName,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
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
        public async Task<List<DTO_StartEndTime>> GetFullBookedHours(int centerId, int resourcesTypeId, DateOnly date)
        {
                var activeStatuses = new[]
                {
                    nameof(BookingStatus.pending),// "pending"
                    nameof(BookingStatus.accepted)
                };
            var fullyBooked = await _db.Bookings
                .Where(b =>
                    b.ResourcesTypeId == resourcesTypeId &&
                    b.CenterId == centerId &&
                    b.BookingDate == date &&
                    activeStatuses.Contains(b.Status))
                .GroupBy(b => new
                {
                    b.StartTime,
                    b.EndTime,
                    b.ResourcesTypeId,
                    b.BookingDate,
                    b.ResourcesType.TotalQuantity
                })
                .Where(g => g.Count() >= g.Key.TotalQuantity)
                .Select(g => new DTO_StartEndTime
                {
                    StartTime = g.Key.StartTime,
                    EndTime = g.Key.EndTime
                })
                .ToListAsync();

            return fullyBooked;
        }
        public async Task<int> GetBookedQuantityForTimeSlot(int resourcesTypeId, DateOnly date, TimeOnly startTime, TimeOnly? endTime)
        {
            // No try/catch here on purpose: a DB failure must not become a fake result (false/0/-1).
            // The caller (AddIfAvailable) handles it: rollback, log, return Error.
            var activeStatuses = new[]
                {
                    nameof(BookingStatus.pending),
                    nameof(BookingStatus.accepted)
                };
           
                var end = endTime ?? startTime.AddHours(1);
                return await _db.Bookings
                    .Where(b =>
                        b.ResourcesTypeId == resourcesTypeId &&
                        b.BookingDate == date &&
                        activeStatuses.Contains(b.Status) &&
                        (b.StartTime < end &&
                        (b.EndTime ?? b.StartTime.AddHours(1)) > startTime))
                    .SumAsync(b => (int?)b.Quantity) ?? 0;
        }
        public async Task<int> GetBookedQuantityForTimeSlotExcludingBooking(int resourcesTypeId, DateOnly date, TimeOnly startTime, TimeOnly? endTime, int excludeBookingId)
        {
            try
            {
                var reqEnd = endTime ?? startTime.AddHours(1);

                return await _db.Bookings
                    .Where(b =>
                        b.ResourcesTypeId == resourcesTypeId &&
                        b.BookingDate == date &&
                        b.Status != nameof(BookingStatus.accepted) &&
                        b.BookingId != excludeBookingId &&
                        (b.StartTime < reqEnd &&
                         (b.EndTime ?? b.StartTime.AddHours(1)) > startTime))
                    .SumAsync(b => (int?)b.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog( "Get Booked Quantity For Time Slot Excluding Booking Error", ex);
                return 0;
            }
        }
        // ================ Read By ================


        // ================ Owner Booking Management ================
        public async Task<bool> Accept_RejectBooking(int bookingId, string status)
        {
            if (status != nameof(BookingStatus.cancelled) || status != nameof(BookingStatus.accepted))
                return false;
            try
            {
                var booking = await _db.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                if (booking == null)
                    return false;
                booking.Status = status.ToLower();
                booking.UpdatedAt = DateTime.UtcNow;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Booking Status Error", ex);
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
                EventLog_Helper.WriteEventLog("Get Bookings By Center Error", ex);
                return new List<DTO_BookingDetails>();
            }
        }
        // ================ Owner Booking Management ================



        // helper methods For (User Booking Check)
        // No try/catch here on purpose: a DB failure must not become a fake result (false/0/-1).
        // The caller (AddIfAvailable) handles it: rollback, log, return Error.
        private async Task<bool> HasDuplicateBookingInternal(Booking booking)
        {
            var activeStatuses = new[]{nameof(BookingStatus.pending), nameof(BookingStatus.accepted) };
            var start = booking.StartTime;
            var end = booking.EndTime ?? start.AddHours(1);
            return await _db.Bookings.AnyAsync(b =>
                b.UserId == booking.UserId &&
                b.BookingDate == booking.BookingDate &&
                activeStatuses.Contains(b.Status) &&
                b.StartTime < end &&
                (b.EndTime ?? b.StartTime.AddHours(1)) > start);
        }
        private async Task<int> CountActiveBookings(int? userId)
        {
            var activeStatuses = new[] { nameof(BookingStatus.pending), nameof(BookingStatus.accepted) };
                return await _db.Bookings
                    .Where(b =>
                        b.UserId == userId &&
                        activeStatuses.Contains(b.Status))
                    .CountAsync();
        }
    }
}