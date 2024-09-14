namespace Domain.Exceptions.Returning
{
    public class InvoiceIsInCartStateException : BadRequestException
    {
        public InvoiceIsInCartStateException(long invoiceId) : base($"Invoice {invoiceId} currently at Cart State!")
        {
        }
    }
}
