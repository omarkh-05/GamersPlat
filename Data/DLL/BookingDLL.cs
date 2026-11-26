using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Booking;
using Domain.DTOs.Invoice;
using Domain.DTOs.Player;
using Domain.DTOs.Pagination;
using Data.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Data;

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
        private static readonly string[] ResourceOccupyingStatuses = {
            nameof(BookingStatus.pending),
            "updated",
            nameof(BookingStatus.accepted),
            "running"
        };
        public enum AddBookingStatus
        {
            Success,
            SlotUnavailable,
            DuplicateBooking,
            LimitReached,
            Error
        }
        private readonly record struct BookingInterval(DateOnly BookingDate, TimeOnly StartTime, TimeOnly EndTime, int Quantity);
        private readonly record struct TimeInterval(long StartTicks, long EndTicks);
        private readonly record struct OccupiedInterval(TimeInterval Interval, int Quantity);
        private readonly record struct CapacitySegment(long StartTicks, long EndTicks, long Quantity);
        private readonly record struct BookingCapacitySnapshot(DateOnly BookingDate, TimeOnly StartTime, TimeOnly EndTime, int Quantity, int Capacity);
        private readonly record struct BookingInvoiceSnapshot(int BookingId, DateTime? StartedAt, decimal UnitPrice, decimal TotalPrice, string ResourceName);
        private readonly record struct InvoicePricing(decimal Subtotal, string? DiscountType, decimal? DiscountValue, decimal TotalPrice);
        public readonly record struct AddBookingResult(AddBookingStatus Status, int BookingId = 0)
        {
            public bool IsSuccess => Status == AddBookingStatus.Success;
        }

        private readonly GamersPlatDbContext _db;
        public BookingDLL(GamersPlatDbContext db)
        {
            _db = db;
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

                var booked = await GetBookedQuantityForTimeSlot(booking.CenterResourceId, booking.BookingDate, booking.StartTime, booking.EndTime!);

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
        public async Task<bool> UpdateIfAvailable(Booking existing, int totalQuantity, string expectedStatus)
        {
            var tx = _db.Database.CurrentTransaction;
            var ownsTransaction = tx == null;

            try
            {
                if (ownsTransaction)
                {
                    tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
                }

                var booked = await GetBookedQuantityForTimeSlotExcludingBooking(existing.CenterResourceId, existing.BookingDate, existing.StartTime, existing.EndTime!, existing.BookingId);

                if (booked + existing.Quantity > totalQuantity)
                {
                    if (ownsTransaction)
                        await tx!.RollbackAsync();

                    return false;
                }

                var updated = await _db.Bookings
                    .Where(b => b.BookingId == existing.BookingId
                        && b.UserId == existing.UserId
                        && b.Status == expectedStatus)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.BookingDate, existing.BookingDate)
                        .SetProperty(b => b.StartTime, existing.StartTime)
                        .SetProperty(b => b.EndTime, existing.EndTime)
                        .SetProperty(b => b.UnitPrice, existing.UnitPrice)
                        .SetProperty(b => b.Subtotal, existing.Subtotal)
                        .SetProperty(b => b.TotalPrice, existing.TotalPrice)
                        .SetProperty(b => b.Status, "updated"));

                if (updated == 0)
                {
                    if (ownsTransaction)
                        await tx!.RollbackAsync();

                    return false;
                }

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

                _db.Entry(existing).State = EntityState.Detached;
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
        public async Task<bool> Cancel(int bookingId, string expectedStatus, string cancellationReason)
        {
            try
            {
                return await _db.Bookings
                    .Where(b => b.BookingId == bookingId
                        && b.Status == expectedStatus
                        && (b.Status == nameof(BookingStatus.pending)
                            || b.Status == nameof(BookingStatus.accepted)))
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Status, nameof(BookingStatus.cancelled))
                        .SetProperty(b => b.CancellationReason, cancellationReason)) > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Cancel Booking Error", ex);
                return false;
            }
        }
        public async Task<PaginatedResponse<DTO_BookingDetails>> GetAll(PaginationParameters pagination)
        {
            try
            {
                return await _db.Bookings
                    .AsNoTracking()
                    .OrderByDescending(b => b.BookingDate)
                    .Select(b => new DTO_BookingDetails
                    {
                        BookingId = b.BookingId,
                        CenterName = b.Center.CenterName,
                        CustomerName = b.CustomerName,
                        PhoneNumber = b.PhoneNumber,
                        ResourcesType = b.CenterResource.ResourcesType.ResourceName + " - " + b.CenterResource.RoomType,
                        BookingDate = b.BookingDate,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        Duration = FormatBookingDuration(b.StartTime, b.EndTime),
                        TotalPrice = b.TotalPrice,
                        Status = b.Status
                    })
                    .ToPaginatedResponseAsync(pagination);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Bookings Error", ex);
                var normalized = pagination.Normalize();
                return PaginatedResponse<DTO_BookingDetails>.Create(Array.Empty<DTO_BookingDetails>(), normalized.PageNumber, normalized.PageSize, 0);
            }
        }
        public async Task<List<DTO_BookingDetails>> GetAll()
        {
            try
            {
                return await _db.Bookings
                    .AsNoTracking()
                    .Select(b => new DTO_BookingDetails
                    {
                        BookingId = b.BookingId,
                        CenterName = b.Center.CenterName,
                        CustomerName = b.CustomerName,
                        PhoneNumber = b.PhoneNumber,
                        ResourcesType = b.CenterResource.ResourcesType.ResourceName + " - " + b.CenterResource.RoomType,
                        BookingDate = b.BookingDate,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        Duration = FormatBookingDuration(b.StartTime, b.EndTime),
                        TotalPrice = b.TotalPrice,
                        Status = b.Status
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get All Bookings Error", ex);
                return new List<DTO_BookingDetails>();
            }
        }
        public async Task<List<int>?> GetExpiredRunningBookingIds()
        {
            var now = DateTime.Now;
            try
            {
                var bookings = await _db.Bookings
                .Where(b => b.Status == "running" && b.StartedAt != null && b.TimeExpiredNotifiedAt == null)
                .Select(b => new
                {
                    b.BookingId,
                    b.StartedAt,
                    b.StartTime,
                    b.EndTime
                })
                .ToListAsync();

                return bookings.Where(b =>
                    {
                        var duration = b.EndTime - b.StartTime;

                        if (duration < TimeSpan.Zero)
                            duration = duration.Add(TimeSpan.FromDays(1));

                        var expectedEndAt = b.StartedAt!.Value.Add(duration);

                        return expectedEndAt <= now;
                    })
                    .Select(b => b.BookingId)
                    .ToList();
            }catch(Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Expired Running Booking Ids Error: ", ex);
                return null;
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
        public async Task<PaginatedResponse<DTO_PlayerBookingsInfo>> GetByUserId(int userId, PaginationParameters pagination)
        {
            try
            {
                return await _db.Bookings
                    .Where(b => b.UserId == userId)
                    .AsNoTracking()
                    .OrderByDescending(b => b.BookingDate)
                    .Select(b => new DTO_PlayerBookingsInfo
                    {
                        BookingId = b.BookingId,
                        CenterName = b.Center.CenterName,
                        Resource = b.CenterResource.ResourcesType.ResourceName,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        OfferId = b.OfferId,
                        Quantity = b.Quantity,
                        TotalPrice = b.TotalPrice,
                        EarnedPoints = b.EarnedPoints,
                        Status = b.Status,
                        BookingDate = b.BookingDate
                    })
                    .ToPaginatedResponseAsync(pagination);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Bookings By UserID Error", ex);
                var normalized = pagination.Normalize();
                return PaginatedResponse<DTO_PlayerBookingsInfo>.Create(
                    Array.Empty<DTO_PlayerBookingsInfo>(), normalized.PageNumber, normalized.PageSize, 0);
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
                        Resource = b.CenterResource.ResourcesType.ResourceName,
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
        public async Task<List<DTO_StartEndTime>> GetFullBookedHours(int centerId, int centerResourceId, DateOnly date)
        {
            var bookings = await GetOccupiedBookingQuery(date, includeNextDay: false)
                .Where(b => b.CenterId == centerId && b.CenterResourceId == centerResourceId)
                .Select(b => new BookingCapacitySnapshot(
                    b.BookingDate,
                    b.StartTime,
                    b.EndTime,
                    b.Quantity,
                    b.CenterResource.TotalQuantity))
                .ToListAsync();

            if (bookings.Count == 0 || bookings[0].Capacity <= 0)
                return new List<DTO_StartEndTime>();

            var intervals = bookings
                .Select(b => new BookingInterval(b.BookingDate, b.StartTime, b.EndTime, b.Quantity))
                .ToArray();
            var capacitySegments = GetCapacitySegments(intervals, date, 0, TimeSpan.TicksPerDay);
            var fullyBookedIntervals = new List<TimeInterval>();

            foreach (var segment in capacitySegments.Where(segment => segment.Quantity >= bookings[0].Capacity))
            {
                if (fullyBookedIntervals.Count > 0 && fullyBookedIntervals[^1].EndTicks == segment.StartTicks)
                {
                    fullyBookedIntervals[^1] = fullyBookedIntervals[^1] with { EndTicks = segment.EndTicks };
                }
                else
                {
                    fullyBookedIntervals.Add(new TimeInterval(segment.StartTicks, segment.EndTicks));
                }
            }

            var result = new List<DTO_StartEndTime>();
            foreach (var interval in fullyBookedIntervals)
            {
                if (interval.StartTicks == 0 && interval.EndTicks == TimeSpan.TicksPerDay)
                {
                    var midday = TimeOnly.FromTimeSpan(TimeSpan.FromHours(12));
                    result.Add(new DTO_StartEndTime { StartTime = TimeOnly.MinValue, EndTime = midday });
                    result.Add(new DTO_StartEndTime { StartTime = midday, EndTime = TimeOnly.MinValue });
                    continue;
                }

                result.Add(new DTO_StartEndTime
                {
                    StartTime = new TimeOnly(interval.StartTicks),
                    EndTime = interval.EndTicks == TimeSpan.TicksPerDay
                        ? TimeOnly.MinValue
                        : new TimeOnly(interval.EndTicks)
                });
            }

            return result;
        }
        public async Task<int> GetBookedQuantityForTimeSlot(int resourcesTypeId, DateOnly date, TimeOnly startTime, TimeOnly? endTime)
        {
            // No try/catch here on purpose: a DB failure must not become a fake result (false/0/-1).
            // The caller (AddIfAvailable) handles it: rollback, log, return Error.
            var end = endTime ?? startTime.AddHours(1);
            var requestedEnd = GetIntervalEndTicks(startTime, end);
            var bookings = await GetOccupiedBookingQuery(date, includeNextDay: true)
                .Where(b => b.CenterResourceId == resourcesTypeId)
                .Select(b => new BookingInterval(b.BookingDate, b.StartTime, b.EndTime, b.Quantity))
                .ToListAsync();

            return GetPeakBookedQuantity(bookings, date, startTime.Ticks, requestedEnd);
        }
        public async Task<int> GetBookedQuantityForTimeSlotExcludingBooking(int resourcesTypeId, DateOnly date, TimeOnly startTime, TimeOnly? endTime, int excludeBookingId)
        {
            var reqEnd = endTime ?? startTime.AddHours(1);
            var requestedEnd = GetIntervalEndTicks(startTime, reqEnd);
            var bookings = await GetOccupiedBookingQuery(date, includeNextDay: true)
                .Where(b => b.CenterResourceId == resourcesTypeId && b.BookingId != excludeBookingId)
                .Select(b => new BookingInterval(b.BookingDate, b.StartTime, b.EndTime, b.Quantity))
                .ToListAsync();

            return GetPeakBookedQuantity(bookings, date, startTime.Ticks, requestedEnd);
        }
        public async Task<List<int>> GetBookingsReadyForAutoCancellation()
        {
            try
            {
                var now = DateTime.Now;
                var cutoffTime = now.AddMinutes(-15);

                var bookings = await _db.Bookings
                    .Where(b => b.Status == "accepted" && b.StartedAt == null)
                    .Select(b => new
                    {
                        b.BookingId,
                        b.BookingDate,
                        b.StartTime
                    })
                    .ToListAsync();

                return bookings
                    .Where(b =>
                    {
                        var scheduledStart = b.BookingDate.ToDateTime(b.StartTime);
                        return scheduledStart <= cutoffTime;
                    })
                    .Select(b => b.BookingId)
                    .ToList();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Bookings Ready For Auto Cancellation Error: ", ex);

                return new List<int>();
            }
        }

        // ================ Read By ================


        // ================ Owner Booking Management ================
        public async Task<bool> AcceptBooking(int bookingId, int userId)
        {
            try
            {
                var now = DateTime.Now;
                return await _db.Bookings
                    .Where(b => b.BookingId == bookingId
                        && (b.Status == nameof(BookingStatus.pending) || b.Status == "updated"))
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Status, nameof(BookingStatus.accepted))
                        .SetProperty(b => b.IsApproved, true)
                        .SetProperty(b => b.AcceptedBy, userId)
                        .SetProperty(b => b.UpdatedAt, now)) > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Accept Booking Status Error", ex);
                return false;
            }
        }
        public async Task<bool> RejectBooking(int bookingId, int userId)
        {
            try
            {
                var now = DateTime.Now;
                return await _db.Bookings
                    .Where(b => b.BookingId == bookingId
                        && (b.Status == nameof(BookingStatus.pending) || b.Status == "updated"))
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Status, nameof(BookingStatus.rejected))
                        .SetProperty(b => b.IsApproved, false)
                        .SetProperty(b => b.RejectedBy, userId)
                        .SetProperty(b => b.UpdatedAt, now)) > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Reject Booking Status Error", ex);
                return false;
            }
        }
        public async Task<bool> StartBooking(int bookingId , int userId)
        {
            try
            {
                var now = DateTime.Now;
                return await _db.Bookings
                    .Where(b => b.BookingId == bookingId && b.Status == nameof(BookingStatus.accepted))
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Status, "running")
                        .SetProperty(b => b.StartedBy, userId)
                        .SetProperty(b => b.StartedAt, now)
                        .SetProperty(b => b.TimeExpiredNotifiedAt, (DateTime?)null)
                        .SetProperty(b => b.UpdatedAt, now)) > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Accept Booking Status Error", ex);
                return false;
            }
        }
        public async Task<DTO_Invoice?> CloseBooking(int bookingId, int userId, DTO_EndBookingInvoice endBooking)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var claimed = await _db.Bookings
                    .Where(b => b.BookingId == bookingId && b.Status == "running" && b.StartedAt != null)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Status, "completed")
                        .SetProperty(b => b.StoppedBy, userId));

                if (claimed == 0)
                {
                    var state = await _db.Bookings
                        .AsNoTracking()
                        .Where(b => b.BookingId == bookingId)
                        .Select(b => new { b.Status, b.StartedAt })
                        .FirstOrDefaultAsync();

                    if (state == null)
                        return null;

                    if (state.Status != "running")
                        throw new InvalidOperationException("Only running bookings can be stopped.");
                    throw new InvalidOperationException("The running booking has no start time.");
                }

                var booking = await _db.Bookings
                    .AsNoTracking()
                    .Where(b => b.BookingId == bookingId)
                    .Select(b => new BookingInvoiceSnapshot(b.BookingId, b.StartedAt, b.UnitPrice, b.TotalPrice, b.CenterResource.ResourcesType.ResourceName))
                    .FirstOrDefaultAsync();

                if (booking == null)
                    throw new InvalidOperationException("The booking could not be loaded after stopping.");

                if (booking.StartedAt == null)
                    throw new InvalidOperationException("The running booking has no start time.");

                if (await _db.Invoices.AnyAsync(i => i.BookingId == bookingId))
                    throw new InvalidOperationException("An invoice already exists for this booking.");

                var endedAt = DateTime.Now;
                var pricing = CalculateInvoicePricing( booking.StartedAt, booking.UnitPrice, booking.TotalPrice, endBooking, endedAt);

                var invoice = new Invoice
                {
                    BookingId = booking.BookingId,
                    StartedAt = booking.StartedAt.Value,
                    EndedAt = endedAt,
                    UnitPrice = booking.UnitPrice,
                    Subtotal = pricing.Subtotal,
                    DiscountType = pricing.DiscountType,
                    DiscountValue = pricing.DiscountValue,
                    TotalPrice = pricing.TotalPrice,
                    CreatedAt = endedAt
                };

                var finalized = await _db.Bookings
                    .Where(b => b.BookingId == bookingId && b.Status == "completed" && b.StoppedBy == userId && b.EndedAt == null)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.EndedAt, endedAt)
                        .SetProperty(b => b.UpdatedAt, endedAt));

                if (finalized == 0)
                    throw new InvalidOperationException("The booking has already been stopped.");

                _db.Invoices.Add(invoice);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                await transaction.DisposeAsync();

                return new DTO_Invoice
                {
                    InvoiceNumber = invoice.InvoiceId,
                    BookingNumber = booking.BookingId,
                    ResourceBooked = booking.ResourceName,
                    StartedAt = invoice.StartedAt,
                    EndedAt = invoice.EndedAt,
                    UnitPrice = invoice.UnitPrice,
                    Subtotal = invoice.Subtotal,
                    DiscountValue = invoice.DiscountValue,
                    DiscountType = invoice.DiscountType,
                    TotalPrice = invoice.TotalPrice
                };
            }
            catch (Exception ex)
            {
                try { await transaction.RollbackAsync(); } catch { }
                EventLog_Helper.WriteEventLog("Stop Booking Error", ex);
                throw;
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
                        BookingId = b.BookingId,
                        CustomerName = b.CustomerName,
                        PhoneNumber = b.PhoneNumber,
                        CenterName = b.Center.CenterName,
                        ResourcesType =
                            b.CenterResource.ResourcesType.ResourceName +
                            " - " +
                            b.CenterResource.RoomType,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        Duration = FormatBookingDurationInMinutes(b.StartTime, b.EndTime),
                        BookingDate = b.BookingDate,
                        TotalPrice = b.TotalPrice,
                        Status = b.Status,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Bookings By Owner Error",ex);
                return new List<DTO_BookingDetails>();
            }
        }
        public async Task<PaginatedResponse<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId, PaginationParameters pagination)
        {
            try
            {
                return await _db.Bookings
                    .Where(b => b.Center.OwnerUserId == ownerId)
                    .AsNoTracking()
                    .OrderByDescending(b => b.BookingDate)
                    .ThenBy(b => b.BookingId)
                    .Select(b => new DTO_BookingDetails
                    {
                        BookingId = b.BookingId,
                        CustomerName = b.CustomerName,
                        PhoneNumber = b.PhoneNumber,
                        CenterName = b.Center.CenterName,
                        ResourcesType = b.CenterResource.ResourcesType.ResourceName + " - " + b.CenterResource.RoomType,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        Duration = FormatBookingDurationInMinutes(b.StartTime, b.EndTime),
                        StartedAt = b.StartedAt,
                        DurationMinutes = (int)GetBookingDuration(b.StartTime, b.EndTime).TotalMinutes,
                        BookingDate = b.BookingDate,
                        TotalPrice = b.TotalPrice,
                        Status = b.Status
                    })
                    .ToPaginatedResponseAsync(pagination);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Bookings By Owner Error", ex);
                var normalized = pagination.Normalize();
                return PaginatedResponse<DTO_BookingDetails>.Create(
                    Array.Empty<DTO_BookingDetails>(), normalized.PageNumber, normalized.PageSize, 0);
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
                        BookingId = b.BookingId,
                        CustomerName = b.CustomerName,
                        PhoneNumber = b.PhoneNumber,
                        CenterName = b.Center.CenterName,
                        ResourcesType =
                            b.CenterResource.ResourcesType.ResourceName +
                            " - " +
                            b.CenterResource.RoomType,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        Duration = FormatBookingDurationInMinutes(b.StartTime, b.EndTime),
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
        public async Task<PaginatedResponse<DTO_BookingDetails>> GetBookingsByCenterId(int centerId, PaginationParameters pagination)   
        {
            try
            {
                return await _db.Bookings
                    .Where(b => b.CenterId == centerId)
                    .AsNoTracking()
                    .OrderByDescending(b => b.BookingDate)
                    .ThenBy(b => b.BookingId)
                    .Select(b => new DTO_BookingDetails
                    {
                        BookingId = b.BookingId,
                        CustomerName = b.CustomerName,
                        PhoneNumber = b.PhoneNumber,
                        CenterName = b.Center.CenterName,
                        ResourcesType = b.CenterResource.ResourcesType.ResourceName + " - " + b.CenterResource.RoomType,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        Duration = FormatBookingDurationInMinutes(b.StartTime, b.EndTime),
                        StartedAt = b.StartedAt,
                        DurationMinutes = (int)GetBookingDuration(b.StartTime, b.EndTime).TotalMinutes,
                        BookingDate = b.BookingDate,
                        TotalPrice = b.TotalPrice,
                        Status = b.Status
                    })
                    .ToPaginatedResponseAsync(pagination);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get Bookings By Center Error", ex);
                var normalized = pagination.Normalize();
                return PaginatedResponse<DTO_BookingDetails>.Create(
                    Array.Empty<DTO_BookingDetails>(), normalized.PageNumber, normalized.PageSize, 0);
            }
        }
        // ================ Owner Booking Management ================



        // helper methods For (User Booking Check)
        // No try/catch here on purpose: a DB failure must not become a fake result (false/0/-1).
        // The caller (AddIfAvailable) handles it: rollback, log, return Error.
        private async Task<bool> HasDuplicateBookingInternal(Booking booking)
        {
            var existingBookings = await GetOccupiedBookingQuery(booking.BookingDate, includeNextDay: true)
                .Where(b => b.UserId == booking.UserId)
                .Select(b => new BookingInterval(b.BookingDate, b.StartTime, b.EndTime, b.Quantity))
                .ToListAsync();

            var requestedInterval = CreateInterval(booking.BookingDate, booking.StartTime, booking.EndTime, booking.BookingDate);
            return existingBookings.Any(existing =>
            {
                var existingInterval = CreateInterval(existing.BookingDate, existing.StartTime, existing.EndTime, booking.BookingDate);
                return existingInterval.StartTicks < requestedInterval.EndTicks
                    && existingInterval.EndTicks > requestedInterval.StartTicks;
            });
        }
        private IQueryable<Booking> GetOccupiedBookingQuery(DateOnly date, bool includeNextDay)
        {
            var firstDate = date == DateOnly.MinValue ? date : date.AddDays(-1);
            var lastDate = includeNextDay && date < DateOnly.MaxValue ? date.AddDays(1) : date;

            return _db.Bookings
                .AsNoTracking()
                .Where(b => b.BookingDate >= firstDate
                    && b.BookingDate <= lastDate
                    && ResourceOccupyingStatuses.Contains(b.Status));
        }
        private static int GetPeakBookedQuantity(IReadOnlyCollection<BookingInterval> bookings, DateOnly referenceDate, long requestedStartTicks, long requestedEndTicks)
        {
            var segments = GetCapacitySegments(bookings, referenceDate, requestedStartTicks, requestedEndTicks);
            return segments.Count == 0 ? 0 : (int)Math.Min(segments.Max(segment => segment.Quantity), int.MaxValue);
        }
        private static List<CapacitySegment> GetCapacitySegments( IReadOnlyCollection<BookingInterval> bookings, DateOnly referenceDate, long rangeStartTicks, long rangeEndTicks)
        {
            var intervals = bookings
                .Select(booking => new OccupiedInterval(
                    CreateInterval(booking.BookingDate, booking.StartTime, booking.EndTime, referenceDate),
                    booking.Quantity))
                .Where(occupied => occupied.Interval.StartTicks < rangeEndTicks
                    && occupied.Interval.EndTicks > rangeStartTicks)
                .ToArray();

            var boundaries = new SortedSet<long> { rangeStartTicks, rangeEndTicks };
            foreach (var occupied in intervals)
            {
                boundaries.Add(Math.Max(occupied.Interval.StartTicks, rangeStartTicks));
                boundaries.Add(Math.Min(occupied.Interval.EndTicks, rangeEndTicks));
            }

            var points = boundaries.ToArray();
            var segments = new List<CapacitySegment>(Math.Max(0, points.Length - 1));
            for (var i = 0; i < points.Length - 1; i++)
            {
                var segmentStart = points[i];
                var segmentEnd = points[i + 1];
                if (segmentStart == segmentEnd)
                    continue;

                var occupiedQuantity = intervals
                    .Where(occupied => occupied.Interval.StartTicks < segmentEnd
                        && occupied.Interval.EndTicks > segmentStart)
                    .Sum(occupied => (long)occupied.Quantity);
                segments.Add(new CapacitySegment(segmentStart, segmentEnd, occupiedQuantity));
            }

            return segments;
        }
        private static long GetIntervalEndTicks(TimeOnly startTime, TimeOnly endTime)
            => endTime.Ticks + (endTime < startTime ? TimeSpan.TicksPerDay : 0);
        private static TimeInterval CreateInterval(DateOnly intervalDate, TimeOnly startTime, TimeOnly endTime, DateOnly referenceDate)
        {
            var startTicks = (long)(intervalDate.DayNumber - referenceDate.DayNumber) * TimeSpan.TicksPerDay + startTime.Ticks;
            var endTicks = (long)(intervalDate.DayNumber - referenceDate.DayNumber) * TimeSpan.TicksPerDay + endTime.Ticks;
            if (endTime < startTime)
                endTicks += TimeSpan.TicksPerDay;

            return new TimeInterval(startTicks, endTicks);
        }
        private static TimeSpan GetBookingDuration(TimeOnly startTime, TimeOnly endTime)
        {
            var duration = endTime - startTime;
            return duration < TimeSpan.Zero ? duration + TimeSpan.FromDays(1) : duration;
        }
        private static string FormatBookingDuration(TimeOnly startTime, TimeOnly endTime)
            => GetBookingDuration(startTime, endTime).ToString();
        private static string FormatBookingDurationInMinutes(TimeOnly startTime, TimeOnly endTime)
            => $"{GetBookingDuration(startTime, endTime).TotalMinutes} Minutes";
        private async Task<int> CountActiveBookings(int? userId)
        {
            return await _db.Bookings
                .Where(b =>
                    b.UserId == userId &&
                    ResourceOccupyingStatuses.Contains(b.Status))
                .CountAsync();
        }


        // Helpers
        public decimal CalculateActualPlayingTimePrice(Booking booking, DateTime endedAt)
        {
            return CalculateActualPlayingTimePrice(booking.StartedAt, booking.UnitPrice, endedAt);
        }
        private static decimal CalculateActualPlayingTimePrice(DateTime? startedAt, decimal unitPrice, DateTime endedAt)
        {
            if (startedAt == null || endedAt < startedAt.Value)
                throw new InvalidOperationException("The booking start time is invalid.");

            var durationHours = (decimal)(endedAt - startedAt.Value).TotalHours;
            return Math.Round(durationHours * unitPrice, 2);
        }
        private static InvoicePricing CalculateInvoicePricing(DateTime? startedAt, decimal unitPrice, decimal originalPrice, DTO_EndBookingInvoice request, DateTime endedAt)
        {
            var subtotal = request.UseOriginalPrice
                ? originalPrice
                : CalculateActualPlayingTimePrice(startedAt, unitPrice, endedAt);

            var discountType = string.IsNullOrWhiteSpace(request.DiscountType)
                ? null
                : request.DiscountType.Trim().ToUpperInvariant();
            var discountValue = request.DiscountValue;

            if (discountType == null && discountValue == null)
                return new InvoicePricing(subtotal, null, null, subtotal);

            if (discountType == null || discountValue == null || discountValue < 0)
                throw new ArgumentException("A valid discount type and non-negative discount value are required.");

            discountValue = Math.Round(discountValue.Value, 2);

            decimal discountAmount;
            if (discountType == "%")
            {
                if (discountValue > 100)
                    throw new ArgumentException("Percentage discounts cannot exceed 100%.");

                discountAmount = Math.Round(subtotal * discountValue.Value / 100m, 2);
            }
            else if (discountType == "JD")
            {
                discountAmount = discountValue.Value;
            }
            else
            {
                throw new ArgumentException("Discount type must be '%' or 'JD'.");
            }

            if (discountAmount > subtotal)
                throw new ArgumentException("Discount cannot exceed the subtotal.");

            return new InvoicePricing(subtotal, discountType, discountValue, Math.Round(subtotal - discountAmount, 2));
        }
        public async Task<bool> TryMarkTimeExpiredNotified(int bookingId)
        {
            var now = DateTime.Now;

            try {
            var affectedRows = await _db.Bookings
                .Where(b => b.BookingId == bookingId && b.Status == "running" && b.TimeExpiredNotifiedAt == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.TimeExpiredNotifiedAt, now));

            return affectedRows > 0;
            }catch(Exception ex)
            {
                EventLog_Helper.WriteEventLog("Try Mark Time Expired Notified Error: ", ex);
                throw;
            }
        }
        public async Task<bool> CancelUnstartedBooking(int bookingId)
        {
            try
            {
                var now = DateTime.Now;

                var affectedRows = await _db.Bookings
                    .Where(b =>  b.BookingId == bookingId &&  b.Status == "accepted" &&  b.StartedAt == null)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Status, "cancelled")
                        .SetProperty(b => b.CancelledAt, now)
                        .SetProperty(b => b.CancellationReason, "Automatically cancelled because the booking was not started within 15 minutes.")
                        .SetProperty(b => b.UpdatedAt, now));

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Cancel Unstarted Booking Error: ", ex);

                return false;
            }
        }
    }
}