﻿using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class InvoiceService
{
    private readonly IApplicationDbContext _uow;

    public InvoiceService(IApplicationDbContext uow)
    {
        _uow = uow;
    }
    
    public IQueryable<Invoice> GetInvoices(int userId, InvoiceState state)
    {
        return _uow.Set<Invoice>()
            .Where(i => i.UserId == userId && i.State == state);
    } 
    
    public Task<bool> IsInvoiceExist(long invoiceId)
    {
        return _uow.Set<Invoice>()
            .AnyAsync(i => i.Id == invoiceId);
    }
}