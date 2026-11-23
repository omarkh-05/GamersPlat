using Data.DLL;
using Domain.DTOs.Invoice;
using Domain.DTOs.Pagination;

namespace Bussiness.BLL
{
    public class InvoiceBLL
    {
        private readonly InvoiceDLL _invoice;

        public InvoiceBLL(InvoiceDLL invoice)
        {
            _invoice = invoice;
        }
        public async Task<PaginatedResponse<DTO_CenterInvoices>?> GetAllCenterInvoices(int centerId, PaginationParameters pagination)
        {
            var invoices = await _invoice.GetCenterInvoices(centerId, pagination);
            if (invoices != null)
                return invoices;
            return null;
        }
    }
}
