namespace CommerceOrders.Domain.Exceptions;

public class EmptyInvoiceException : BadRequestException
{
    public EmptyInvoiceException(long invoiceId) : base($"Invoice {invoiceId} is Empty!")
    {
    }
}