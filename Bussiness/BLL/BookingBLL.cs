using Data;
using DataLayer;
using Domain.DTOs.Booking;
using Domain.DTOs.Player;
using System;
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
                var bookedForSlot = await BookingDLL.GetBookedQuantityForTimeSlot(addBooking.ResourcesTypeId, addBooking.BookingDate, start, end);
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
                    var offer = await DataLayer.OfferDLL.GetByID(addBooking.OfferId.Value);
                    if (offer != null && offer.IsActive && offer.DiscountValue.HasValue && !string.IsNullOrWhiteSpace(offer.DiscountType))
                    {
                        // Validate offer applicability by date/time if dates are present on the offer
                        var bookingDateTime = addBooking.BookingDate.ToDateTime(start);
                        if ((offer.StartDate == default || offer.StartDate <= bookingDateTime) &&
                            (offer.EndDate == default || offer.EndDate >= bookingDateTime))
                        {
                            discountType = offer.DiscountType;
                            discountValue = offer.DiscountValue;
                            if (offer.DiscountType.Equals("percentage", StringComparison.OrdinalIgnoreCase))
                            {
                                var disc = subtotal * (decimal)(offer.DiscountValue.Value / 100.0);
                                totalPrice = Math.Round(subtotal - disc, 2);
                            }
                            else
                            {
                                var disc = (decimal)offer.DiscountValue.Value;
                                totalPrice = Math.Round(Math.Max(0, subtotal - disc), 2);
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
                var existing = await BookingDLL.GetByID(updaetBooking.BookingId);

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

                // Prevent overlapping bookings for the same resource type/time slot (exclude this booking)
                var bookedForSlot = await BookingDLL.GetBookedQuantityForTimeSlotExcludingBooking(existing.ResourcesTypeId, updaetBooking.BookingDate, start, end, updaetBooking.BookingId);
                var requestedQuantity = updaetBooking.Quantity > 0 ? updaetBooking.Quantity : existing.Quantity;
                // Ensure resources type info available
                var rt = existing.ResourcesType ?? await _resourcesTypeBLL.GetByID(existing.ResourcesTypeId);
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
                var subtotal = Math.Round(unitPrice * requestedQuantity * durationHours, 2);

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
                    var offer = await DataLayer.OfferDLL.GetByID(existing.OfferId.Value);
                    if (offer != null && offer.IsActive && offer.DiscountValue.HasValue && !string.IsNullOrWhiteSpace(offer.DiscountType))
                    {
                        if (offer.DiscountType.Equals("percentage", StringComparison.OrdinalIgnoreCase))
                        {
                            var disc = subtotal * (decimal)(offer.DiscountValue.Value / 100.0);
                            totalPrice = Math.Round(subtotal - disc, 2);
                        }
                        else
                        {
                            var disc = (decimal)offer.DiscountValue.Value;
                            totalPrice = Math.Round(Math.Max(0, subtotal - disc), 2);
                        }
                    }
                }

                existing.TotalPrice = totalPrice;

                return await BookingDLL.Update(existing);
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
        public async Task<List<DTO_PlayerBookingsInfo>> GetByPlayerId(int userId) => await BookingDLL.GetByUserId(userId);
        public async Task<Booking?> GetByID(int bookingID) => await BookingDLL.GetByID(bookingID);
        // ================ Read By ================


        // ================ Owner Booking Managament ================
        public async Task<bool> Accept_RejectBooking(int bookingId,string status) => await BookingDLL.Accept_RejectBooking(bookingId,status);
        public async Task<List<DTO_BookingDetails>> GetBookingsByOwnerId(int ownerId) => await BookingDLL.GetBookingsByOwnerId(ownerId);
        public async Task<List<DTO_BookingDetails>> GetBookingsByCenterId(int centerId) => await BookingDLL.GetBookingsByCenterId(centerId);
        // ================ Owner Booking Managament ================
    }
}
