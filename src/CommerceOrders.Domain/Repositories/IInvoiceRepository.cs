using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        IEnumerable<Invoice?> GetInvoices();
        
        IEnumerable<InvoiceItem> GetInvoiceItems();

        IEnumerable<Invoice?> GetInvoiceByState(int userId, InvoiceState invoiceState);

        Task<Invoice> GetInvoiceById(long id);

        Invoice GetCartOfUser(int userId);
        
        Task<Invoice> InsertInvoice(Invoice invoice);

        Invoice UpdateInvoice(Invoice invoice);

        Task<InvoiceItem> GetProductOfInvoice(long invoiceId, int productId);

        Invoice GetSecondCartOfUser(int userId);

        Task<IEnumerable<InvoiceItem>> GetNotDeleteItems(long invoiceId);
    }
}