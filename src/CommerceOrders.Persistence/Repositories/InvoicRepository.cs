using CommerceOrders.Domain.Exceptions;
using CommerceOrders.Domain.Repositories;
using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Persistence.Repositories;

internal class InvoiceRepository : IInvoiceRepository
{
    private readonly InvoiceDbContext _dbContext;

    public InvoiceRepository(InvoiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Invoice?> GetInvoices() => _dbContext.Invoices;

    public IEnumerable<InvoiceItem> GetInvoiceItems() => _dbContext.InvoiceItems;

    public async Task<Invoice> GetInvoiceById(long id)
    {
        var invoice = await _dbContext.Invoices.Include(invoice => invoice.InvoiceItems)
            .SingleOrDefaultAsync(invoice => invoice.Id == id);

        if (invoice is null)
        {
            throw new InvoiceNotFoundException(id);
        }

        return invoice;
    }
    
    public Invoice UpdateInvoice(Invoice invoice)
    {
        _dbContext.Invoices.Attach(invoice);
        _dbContext.Entry(invoice).State = EntityState.Modified;

        return invoice;
    }

    public IEnumerable<Invoice?> GetInvoiceByState(int userId, InvoiceState invoiceState)
    {
        return _dbContext.Invoices.Include(invoice => invoice.InvoiceItems)
            .Where(invoice => invoice.UserId == userId &&
                              invoice.State == invoiceState).AsEnumerable();
        
    }

    public async Task<InvoiceItem> GetProductOfInvoice(long invoiceId, int productId)
    {
        var invoice = await GetInvoiceById(invoiceId);
        var invoiceItem = invoice.InvoiceItems.SingleOrDefault(invoiceItem => invoiceItem.ProductId == productId);

        if (invoiceItem is null)
        {
            throw new OrderItemNotFoundException(invoiceId, productId);
        }

        return invoiceItem;
    }
}