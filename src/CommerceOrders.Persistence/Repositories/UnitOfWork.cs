using CommerceOrders.Domain.Repositories;

namespace CommerceOrders.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public IInvoiceRepository InvoiceRepository { get; }

    private readonly InvoiceContext _context;

    public UnitOfWork(InvoiceContext context, IInvoiceRepository invoiceRepository)
    {
        _context = context;
        InvoiceRepository = invoiceRepository;
    }



    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}