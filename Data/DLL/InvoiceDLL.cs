using Data.EF;
using Domain.DTOs.Invoice;
using Domain.DTOs.Player;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DLL
{
    public class InvoiceDLL
    {
        private readonly GamersPlatDbContext _db;

        public InvoiceDLL(GamersPlatDbContext db)
        {
            _db = db;
        }
        public async Task<List<DTO_CenterInvoices>> GetCenterInvoices(int centerId)
        {
            try
            {
                return await _db.Invoices
                        .Where(i => i.Booking.CenterId == centerId)
                        .Select(i => new DTO_CenterInvoices
                        {
                            InvoiceNumber = i.InvoiceId,
                            BookingNumber = i.BookingId,
                            CenterName = i.Booking.Center.CenterName,
                            StartedAt = i.StartedAt,
                            EndedAt = i.EndedAt,
                            ResourceBooked = i.Booking.CenterResource.ResourcesType.ResourceName,
                            UnitPrice = i.UnitPrice,
                            Subtotal = i.Subtotal,
                            DiscountValue = i.DiscountValue,
                            DiscountType = i.DiscountType,
                            TotalPrice = i.TotalPrice,

                            AcceptedBy = i.Booking.AcceptedByNavigation.FullName,
                            RejectedBy = i.Booking.RejectedByNavigation.FullName,
                            StartedBy = i.Booking.StartedByNavigation.FullName,
                            StoppedBy = i.Booking.StoppedByNavigation.FullName
                        })
                        .ToListAsync();
            }
            catch(Exception ex)
            {
                EventLog_Helper.WriteEventLog("Error While Getting Owner Invoices :", ex);
                return null;
            }
        }
    }
}
