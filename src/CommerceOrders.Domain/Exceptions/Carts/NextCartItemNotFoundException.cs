namespace CommerceOrders.Domain.Exceptions.Carts;

public class NextCartItemNotFoundException : NotFoundException
{
    public NextCartItemNotFoundException(int userId, int productId) : base(
        $"product {productId} not found in User {userId} next cart ")
    {
    }
}