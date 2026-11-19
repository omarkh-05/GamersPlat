using Data.DLL;
using Domain.DTOs.Invoice;

namespace Bussiness.BLL
{
    public class InvoiceBLL
    {
        private readonly InvoiceDLL _invoice;

        public InvoiceBLL(InvoiceDLL invoice)
        {
            _invoice = invoice;
        }
        public async Task<List<DTO_CenterInvoices>?> GetAllCenterInvoices(int centerId)
        {
            var invoices = await _invoice.GetCenterInvoices(centerId);
            if (invoices != null)
                return invoices;
            return null;
        }
    }
}
