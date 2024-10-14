using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class InvoiceService
{
    private readonly IUnitOfWork _uow;

    public InvoiceService(IUnitOfWork uow)
    {
        _uow = uow;
    }
    
    public IQueryable<Invoice> GetInvoices(int userId, InvoiceState state)
    {
        return _uow.Set<Invoice>()
            .Where(i => i.UserId == userId && i.State == state);
    } 
}