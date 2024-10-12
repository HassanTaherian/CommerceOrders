namespace CommerceOrders.Domain.Exceptions.Order;

public class OrderNotFoundException : NotFoundException
{
    public OrderNotFoundException(long orderId) : base(
        $"Order {orderId} not found!")
    {
    }
}