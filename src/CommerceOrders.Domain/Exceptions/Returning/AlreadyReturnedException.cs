namespace CommerceOrders.Domain.Exceptions.Returning;

public class AlreadyReturnedException : BadRequestException
{
    public AlreadyReturnedException(long invoiceId) : base($"Invoice {invoiceId} already has been returned!")
    {
    }
}
