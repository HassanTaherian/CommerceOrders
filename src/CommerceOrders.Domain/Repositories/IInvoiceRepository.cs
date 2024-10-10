using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Domain.Repositories;

public interface IInvoiceRepository
{
    IEnumerable<Invoice?> GetInvoices();

    IEnumerable<InvoiceItem> GetInvoiceItems();

    IEnumerable<Invoice?> GetInvoiceByState(int userId, InvoiceState invoiceState);

    Task<Invoice> GetInvoiceById(long id);

    Invoice? FetchCart(int userId);

    void Add(Invoice invoice);

    Invoice UpdateInvoice(Invoice invoice);

    Task<InvoiceItem> GetProductOfInvoice(long invoiceId, int productId);

    Task<Invoice?> FetchNextCart(int userId);

    Task<IEnumerable<InvoiceItem>> GetNotDeleteItems(long invoiceId);
    
    Task<Invoice?> FetchCartWithItems(int userId);
}