using Data;
using DataLayer;
using Domain.DTOs.Booking;
using Domain.DTOs.Player;
using System;
using Bussiness.Interfaces;

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

        public BookingBLL(
            IResourcesTypeService resourcesTypeBLL,
            OfferDLL offerDLL,
            BookingDLL bookingDLL)
        {
            _resourcesTypeBLL = resourcesTypeBLL;
            _offerDLL = offerDLL;
            _bookingDLL = bookingDLL;
        }


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

                // Basic validations
                if (!rt.IsActive)
                {
                    LastError = "Resource type is not active";
                    return false;
                }

                if (addBooking.Quantity <= 0)
                {
                    LastError = "Quantity must be at least 1";
                    return false;
                }

                if (addBooking.Quantity > rt.TotalQuantity)
                {
                    LastError = "Requested quantity exceeds total available resources";
                    return false;
                }

                // Determine time range (default to 1 hour if end not provided)
                var start = addBooking.StartTime;
                var end = addBooking.EndTime ?? start.AddHours(1);

                if (end <= start)
                {
                    LastError = "End time must be after start time";
                    return false;
                }

                // Prevent overlapping bookings for the same resource type/time slot
                var bookedForSlot = await _bookingDLL.GetBookedQuantityForTimeSlot(
                    addBooking.ResourcesTypeId,
                    addBooking.BookingDate,
                    start,
                    end);

                if (bookedForSlot + addBooking.Quantity > rt.TotalQuantity)
                {
                    LastError = "Insufficient availability for the requested time slot";
                    return false;
                }

                // Price calculation
                var durationMinutes = (end - start).TotalMinutes;
                var durationHours = (decimal)durationMinutes / 60m;
                var unitPrice = rt.HourlyPrice;
                var quantity = addBooking.Quantity;
                var subtotal = Math.Round(unitPrice * quantity * durationHours, 2);

                // Apply offer if provided
                string? discountType = null;
                double? discountValue = null;
                decimal totalPrice = subtotal;

                if (addBooking.OfferId.HasValue)
                {
                    var offer = await _offerDLL.GetByID(addBooking.OfferId.Value);

                    if (offer != null &&
                        offer.IsActive &&
                        offer.DiscountValue.HasValue &&
                        !string.IsNullOrWhiteSpace(offer.DiscountType))
                    {
                        // Validate offer applicability by date/time if dates are present on the offer
                        var bookingDateTime = addBooking.BookingDate.ToDateTime(start);

                        if ((offer.StartDate == default || offer.StartDate <= bookingDateTime) &&
                            (offer.EndDate == default || offer.EndDate >= bookingDateTime))
                        {
                            discountType = offer.DiscountType;
                            discountValue = offer.DiscountValue;

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
                    }
                }

                var booking = new Booking
                {
                    UserId = addBooking.UserId,
                    CenterId = addBooking.CenterId,
                    ResourcesTypeId = addBooking.ResourcesTypeId,
                    BookingDate = addBooking.BookingDate,
                    StartTime = addBooking.StartTime,
                    EndTime = addBooking.EndTime,
                    Quantity = addBooking.Quantity,
                    UnitPrice = unitPrice,
                    Subtotal = subtotal,
                    DiscountType = discountType,
                    DiscountValue = discountValue,
                    TotalPrice = totalPrice,
                    CustomerName = addBooking.CustomerName,
                    PhoneNumber = addBooking.PhoneNumber,
                    OfferId = addBooking.OfferId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                int bookingID = await _bookingDLL.Add(booking);

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
                var existing = await _bookingDLL.GetByID(updaetBooking.BookingId);

                if (existing == null)
                {
                    LastError = "Booking not found";
                    return false;
                }

                // Determine time range (default to 1 hour if end not provided)
                var start = updaetBooking.StartTime;
                var end = updaetBooking.EndTime ?? start.AddHours(1);

                if (end <= start)
                {
                    LastError = "End time must be after start time";
                    return false;
                }

                // Prevent overlapping bookings for the same resource type/time slot
                // (exclude this booking)
                var bookedForSlot =
                    await _bookingDLL.GetBookedQuantityForTimeSlotExcludingBooking(
                        existing.ResourcesTypeId,
                        updaetBooking.BookingDate,
                        start,
                        end,
                        updaetBooking.BookingId);

                var requestedQuantity =
                    updaetBooking.Quantity > 0
                        ? updaetBooking.Quantity
                        : existing.Quantity;

                // Ensure resources type info available
                var rt = existing.ResourcesType ??
                         await _resourcesTypeBLL.GetByID(existing.ResourcesTypeId);

                if (rt == null)
                {
                    LastError = "Resource type not found for existing booking";
                    return false;
                }

                if (bookedForSlot + requestedQuantity > rt.TotalQuantity)
                {
                    LastError = "Insufficient availability for the requested time slot";
                    return false;
                }

                // Recalculate pricing if times or quantity changed
                var durationMinutes = (end - start).TotalMinutes;
                var durationHours = (decimal)durationMinutes / 60m;
                var unitPrice = rt.HourlyPrice;
                var subtotal = Math.Round(
                    unitPrice * requestedQuantity * durationHours,
                    2);

                existing.BookingDate = updaetBooking.BookingDate;
                existing.StartTime = updaetBooking.StartTime;
                existing.EndTime = updaetBooking.EndTime;
                existing.CustomerName = updaetBooking.CustomerName;
                existing.PhoneNumber = updaetBooking.PhoneNumber;
                existing.Quantity = requestedQuantity;
                existing.UnitPrice = unitPrice;
                existing.Subtotal = subtotal;

                // Recompute total price considering any offer attached
                decimal totalPrice = subtotal;

                if (existing.OfferId.HasValue)
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
                }

                existing.TotalPrice = totalPrice;

                return await _bookingDLL.Update(existing);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public async Task<bool> Delete(int bookingID)
            => await _bookingDLL.Delete(bookingID);

        public async Task<bool> Cancel(int bookingID)
            => await _bookingDLL.Cancel(bookingID);

        public async Task<List<Booking>> GetAll()
            => await _bookingDLL.GetAll();

        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<DTO_PlayerBookingsInfo>> GetByPlayerId(int userId)
            => await _bookingDLL.GetByUserId(userId);

        public async Task<Booking?> GetByID(int bookingID)
            => await _bookingDLL.GetByID(bookingID);

        // ================ Read By ================


        // ================ Owner Booking Management ================
        public async Task<bool> Accept_RejectBooking(int bookingId, string status)
            => await _bookingDLL.Accept_RejectBooking(bookingId, status);

        public async Task<List<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId)
            => await _bookingDLL.GetBookingsByOwnerId(ownerId);

        public async Task<List<DTO_BookingDetails>> GetBookingsByCenterId(int centerId)
            => await _bookingDLL.GetBookingsByCenterId(centerId);

        // Helper to compute booked quantity for availability check
        public async Task<int> GetBookedQuantity(
            int centerId,
            int resourcesTypeId,
            DateOnly date)
        {
            // BookingDLL provides methods to compute booked quantities by resource and date.
            // Use the non-time-slot variant when only date is provided.
            return await _bookingDLL.GetBookedQuantity(
                resourcesTypeId,
                date);
        }

        // ================ Owner Booking Management ================
    }
}