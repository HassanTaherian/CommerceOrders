namespace CommerceOrders.Domain.Exceptions;

public sealed class CartItemQuantityOutOfRangeInputException : BadRequestException
{
    public CartItemQuantityOutOfRangeInputException()
        : base("Cart item quantity must be more than 0")
    {
    }
}