using Bussiness.Interfaces;
using Data;
using Data.DLL;
using Data.EF;
using DataLayer;
using Domain.DTOs.Auth;
using Domain.DTOs.Booking;
using Domain.DTOs.Player;
using Domain.DTOs.Resource;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static DataLayer.BookingDLL;

namespace Bussiness
{
    public class BookingBLL : IBookingService
    {
        public int _bookingID { get; private set; }
        public int LastId => _bookingID;
        public string? LastError { get; private set; }

        private readonly IResourcesTypeService _resourcesTypeBLL;
        private readonly OfferDLL _offerDLL;
        private readonly BookingDLL _bookingDLL;
        private readonly IUser _userBLL;
        private readonly IAuthService _authService;
        private readonly GamersPlatDbContext _db;

        public BookingBLL(IResourcesTypeService resourcesTypeBLL, OfferDLL offerDLL, BookingDLL bookingDLL, IUser userBLL, IAuthService authService, GamersPlatDbContext db)
        {
            _resourcesTypeBLL = resourcesTypeBLL;
            _offerDLL = offerDLL;
            _bookingDLL = bookingDLL;
            _userBLL = userBLL;
            _authService = authService;
            _db = db;
        }


        // ================ CRUD ================
        public async Task<bool> Add(DTO_BookingBase dto, int userId, string? customerName, string? phone)
        {
            try
            {
                var rt = await GetValidResourceType(dto.ResourcesTypeId, dto.CenterId);
                if (rt == null)
                    return false;

                if (!ValidateBookingTime(dto.BookingDate,dto.StartTime,dto.EndTime,out var start,out var end))
                    return false;

                var user = await _userBLL.GetById(userId);
                if (user == null)
                {
                    LastError = "User not found";
                    return false;
                }

                var isStaffOrAdmin = IsStaffOrAdmin(user.Roles);

                var duration = end - start;
                if (duration < TimeSpan.Zero)
                    duration += TimeSpan.FromDays(1);
                var durationHours = (decimal)duration.TotalMinutes / 60m;
                var unitPrice = rt.HourlyPrice;
                var subtotal = Math.Round(unitPrice * durationHours, 2);
                var totalPrice = subtotal;

                string? discountType = null;
                double? discountValue = null;

                var booking = new Booking
                {
                    UserId = userId,
                    ResourcesTypeId = dto.ResourcesTypeId,
                    BookingDate = dto.BookingDate,
                    StartTime = start,
                    EndTime = end,
                    Quantity = 1,
                    UnitPrice = unitPrice,
                    Subtotal = subtotal,
                    DiscountType = discountType,
                    DiscountValue = discountValue,
                    TotalPrice = totalPrice,
                    CustomerName = isStaffOrAdmin ? customerName ?? user.FullName : user.FullName,
                    PhoneNumber = isStaffOrAdmin ? phone ?? user.PhoneNumber : user.PhoneNumber,
                    CenterId = dto.CenterId,
                    Status = "pending",
                    CreatedAt = DateTime.Now
                };

                var result = await _bookingDLL.AddIfAvailable(booking, rt.TotalQuantity, isStaffOrAdmin);

                LastError = result.Status switch
                {
                    AddBookingStatus.Success => null,
                    AddBookingStatus.SlotUnavailable => "Unavailable Booking for the requested time slot",
                    AddBookingStatus.DuplicateBooking => "You already have a booking that overlaps this time slot",
                    AddBookingStatus.LimitReached => "Cannot make more than 5 pending or active bookings",
                    _ => "Could not save booking"
                };

                if (!result.IsSuccess) return false;

                _bookingID = result.BookingId;
                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Booking Add Error", ex);
                LastError = "Unexpected error";
                return false;
            }
        }
        public async Task<GuestBookingResult> CreateGuestBookingAsync(DTO_AddGuestBooking dto)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                if (await _userBLL.GetUserInfoByPhone(dto.PhoneNumber) != null)
                {
                    await transaction.RollbackAsync();
                    return new(false, Message: "Error while registering, try to login", Failure: GuestBookingFailure.ExistingAccount);
                }

                int userId;
                try
                {
                    userId = await _authService.RegisterWithRoleAsync(new UserRegisterRequest
                    {
                        FullName = dto.CustomerName,
                        PhoneNumber = dto.PhoneNumber,
                        Password = dto.Password,
                    }, "Player");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    EventLog_Helper.WriteEventLog("Guest registration error", ex);
                    return new(false, Message: "Error occurred while registering user", Failure: GuestBookingFailure.RegistrationFailed);
                }

                if (!await Add(dto, userId, dto.CustomerName, dto.PhoneNumber))
                {
                    await transaction.RollbackAsync();
                    return new(false, Message: LastError ?? "Booking failed", Failure: GuestBookingFailure.BookingFailed);
                }

                await transaction.CommitAsync();

                TokenResponse? token = null;
                try
                {
                    token = await _authService.LoginAsync(new LoginRequest
                    {
                        PhoneNumber = dto.PhoneNumber,
                        Password = dto.Password
                    });
                }
                catch (Exception ex)
                {
                    EventLog_Helper.WriteEventLog("Guest login error", ex);
                }

                return new(true, LastId, Token: token);
            }
            catch (Exception ex)
            {
                try { await transaction.RollbackAsync(); } catch { }
                EventLog_Helper.WriteEventLog("Guest booking transaction error", ex);
                return new(false, Message: "An error occurred while creating the booking", Failure: GuestBookingFailure.BookingFailed);
            }
        }
        public async Task<bool> Update(DTO_UpdateBooking dto,int userId)
        {
            LastError = null;
            try
            {
                var existing = await GetByID(dto.BookingId);

                if (existing == null)
                {
                    LastError = "Booking not found";
                    return false;
                }

                // Only booking owner can update
                if (!existing.UserId.HasValue || existing.UserId.Value != userId)
                {
                    LastError = "You are not authorized to update this booking";
                    return false;
                }

                if (existing.Status != "pending" && existing.Status != "accepted")
                {
                    LastError = "Only pending or accepted bookings can be updated";
                    return false;
                }

                if (!ValidateBookingTime(dto.BookingDate, dto.StartTime, dto.EndTime, out var start, out var end))
                    return false;

                var rt = await GetValidResourceType(existing.ResourcesTypeId, existing.CenterId);

                if (rt == null)
                {
                    LastError = "Resource type not found for existing booking";
                    return false;
                }

                var duration = end - start;
                if (duration < TimeSpan.Zero)
                    duration += TimeSpan.FromDays(1);
                var durationHours = (decimal)duration.TotalMinutes / 60m;
                var unitPrice = rt.HourlyPrice;
                var subtotal = Math.Round(unitPrice * durationHours, 2);
                existing.BookingDate = dto.BookingDate;
                existing.StartTime = start;
                existing.EndTime = end;
                existing.UnitPrice = unitPrice;
                existing.Subtotal = subtotal;
                existing.TotalPrice = subtotal;

                var updated = await _bookingDLL.UpdateIfAvailable(existing, rt.TotalQuantity);

                if (!updated)
                {
                    LastError = "Requested time slot is not available";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Booking Update Error", ex);
                LastError = "An unexpected error occurred while updating the booking";
                return false;
            }
            /*if (existing.OfferId.HasValue)
            {
                var offer = await _offerDLL.GetByID(existing.OfferId.Value);

                if (offer != null &&
                    offer.IsActive &&
                    offer.DiscountValue.HasValue &&
                    !string.IsNullOrWhiteSpace(offer.DiscountType))
                {
                    if (offer.DiscountType.Equals(
                        "percentage",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var disc = subtotal *
                            (decimal)(offer.DiscountValue.Value / 100.0);

                        totalPrice = Math.Round(subtotal - disc, 2);
                    }
                    else
                    {
                        var disc = (decimal)offer.DiscountValue.Value;

                        totalPrice = Math.Round(
                            Math.Max(0, subtotal - disc),
                            2);
                    }
                }
            }*/
        }
        public async Task<bool> Delete(int bookingID) => await _bookingDLL.Delete(bookingID);
        public async Task<bool> Cancel(DTO_CancelBooking cancelBooking, int userId)
        {
            LastError = null;
            try
            {
                var existing = await _bookingDLL.GetByID(cancelBooking.BookingId);
                if (existing == null)
                {
                    LastError = "Booking not found";
                    return false;
                }
                // Only booking owner can cancel
                if (!existing.UserId.HasValue || existing.UserId.Value != userId)
                {
                    LastError = "You are not authorized to cancel this booking";
                    return false;
                }
                if (existing.Status == nameof(BookingStatus.cancelled))
                {
                    LastError = "Booking is already cancelled";
                    return false;
                }

                if (existing.Status != nameof(BookingStatus.pending) && existing.Status != nameof(BookingStatus.accepted))
                {
                    LastError = "Booking cannot be cancelled in its current status";
                    return false;
                }
                existing.Status = nameof(BookingStatus.cancelled);
                existing.CancellationReason = cancelBooking.CancelReason ?? "Cancelled by user";

                var cancelled = await _bookingDLL.Cancel();
                if (!cancelled)
                {
                    LastError = "Failed to cancel booking";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Cancel Booking Error", ex);
                LastError = "An unexpected error occurred while cancelling the booking";
                return false;
            }
        }
        public async Task<List<Booking>> GetAll() => await _bookingDLL.GetAll();
        // ================ CRUD ================
        
        
        // ================ Read By ================
        public async Task<List<DTO_PlayerBookingsInfo>> GetByPlayerId(int userId) => await _bookingDLL.GetByUserId(userId);
        public async Task<Booking?> GetByID(int bookingID) => await _bookingDLL.GetByID(bookingID);
        public async Task<List<DTO_StartEndTime>?> GetFullBookedHours(int centerId, int resourcesTypeId, DateOnly date)
        {
            if (resourcesTypeId <= 0 || centerId <= 0 || date < DateOnly.FromDateTime(DateTime.Today))
                return null;

            var checkIfResourceForCenter = await _resourcesTypeBLL.CheckCenterAndResource(centerId, resourcesTypeId);
            if (checkIfResourceForCenter == false)
                return null;

            return await _bookingDLL.GetFullBookedHours(centerId,resourcesTypeId, date);
        }
        // ================ Read By ================


        // ================ Owner Booking Management ================
        public async Task<bool> Accept_RejectBooking(int bookingId, string status) => await _bookingDLL.Accept_RejectBooking(bookingId, status);
        public async Task<List<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId) => await _bookingDLL.GetBookingsByOwnerId(ownerId);
        public async Task<List<DTO_BookingDetails>> GetBookingsByCenterId(int centerId) => await _bookingDLL.GetBookingsByCenterId(centerId);
        // ================ Owner Booking Management ================


        // ================ Helpers ================
        private async Task<DTO_ResourceInfo?> GetValidResourceType(int resourcesTypeId,int centerId)
        {
            var rt = await _resourcesTypeBLL.GetResourceInfo(resourcesTypeId);

            if (rt == null)
            {
                LastError = "Resource type not found";
                return null;
            }
            if (!rt.IsActive)
            {
                LastError = "Resource type is not active";
                return null;
            }
            if (rt.CenterId != centerId)
            {
                LastError = "Resource type does not belong to this center";
                return null;
            }
            return rt;
        }
        private bool ValidateBookingTime(DateOnly bookingDate,TimeOnly start,TimeOnly? endTime,out TimeOnly validatedStart,out TimeOnly validatedEnd)
        {
            validatedStart = start;
            validatedEnd = endTime ?? start.AddHours(1);

            if (validatedEnd == validatedStart)
            {
                LastError = "End time must be different from start time";
                return false;
            }

            // حساب المدة مع دعم تجاوز منتصف الليل
            var duration = validatedEnd - validatedStart;

            if (duration < TimeSpan.Zero)
                duration += TimeSpan.FromDays(1);

            if (duration > TimeSpan.FromHours(12))
            {
                LastError = "Booking duration is too long";
                return false;
            }

            var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Amman");
            var nowLocal = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

            var bookingStart = bookingDate.ToDateTime(validatedStart);

            if (bookingStart < nowLocal)
            {
                LastError = "Cannot book in the past";
                return false;
            }

            return true;
        }
        private bool IsStaffOrAdmin(IEnumerable<string>? roles)
        {
            return roles?.Any(r => r != null && (r.Equals("admin", StringComparison.OrdinalIgnoreCase) || r.Equals("staff", StringComparison.OrdinalIgnoreCase))) ?? false;
        }
        // ================ Helpers ================
    }
}