using CommerceOrders.Domain.Repositories;

namespace CommerceOrders.Persistence.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    public IInvoiceRepository InvoiceRepository { get; }

    private readonly InvoiceDbContext _context;

    public UnitOfWork(InvoiceDbContext context, IInvoiceRepository invoiceRepository)
    {
        _context = context;
        InvoiceRepository = invoiceRepository;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}