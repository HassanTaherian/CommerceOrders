namespace CommerceOrders.Domain.Exceptions
{
    public class InvoiceNotFoundException : NotFoundException
    {
        public InvoiceNotFoundException(long invoiceId)
            : base($"The invoice with id {invoiceId} not found!")
        {
        }

        public InvoiceNotFoundException(int userId)
            : base($"User With Id {userId} has no Invoice...")
        {
        }
    }
}