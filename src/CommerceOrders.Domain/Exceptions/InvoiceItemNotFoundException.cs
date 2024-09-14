namespace CommerceOrders.Domain.Exceptions
{
    public class InvoiceItemNotFoundException : NotFoundException
    {
        // Todo: Collecting all error message to a global dictionary
        public InvoiceItemNotFoundException(long invoiceId, int productId) : base($"There is no product {productId} in invoice {invoiceId}")
        {
        }
    }
}
