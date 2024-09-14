namespace CommerceOrders.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public IInvoiceRepository InvoiceRepository { get; }
        Task<bool> SaveChangesAsync();
    }
}