namespace CommerceOrders.Domain.Exceptions;

public class AddressNotSpecifiedException : BadRequestException
{
    public AddressNotSpecifiedException(long invoiceId) : base($"Address hasn't been set for cart {invoiceId}!")
    {
    }
}
