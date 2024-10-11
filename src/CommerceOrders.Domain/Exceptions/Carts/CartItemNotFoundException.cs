namespace CommerceOrders.Domain.Exceptions.Carts;

public class CartItemNotFoundException : NotFoundException
{
    public CartItemNotFoundException(int userId, int productId) : base(
        $"product {productId} not found in User {userId} cart ")
    {
    }
}