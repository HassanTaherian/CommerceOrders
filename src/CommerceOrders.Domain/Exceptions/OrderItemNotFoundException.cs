namespace CommerceOrders.Domain.Exceptions;

public class OrderItemNotFoundException : NotFoundException
{
    // Todo: Collecting all error message to a global dictionary
    public OrderItemNotFoundException(long invoiceId, int productId) : base($"There is no product {productId} in order {invoiceId}")
    {
    }
}
