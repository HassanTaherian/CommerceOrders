namespace CommerceOrders.Domain.Exceptions.Returning;

public class AlreadyReturnedException : BadRequestException
{
    public AlreadyReturnedException(long invoiceId) : base($"Order {invoiceId} already has been returned!")
    {
    }
}
