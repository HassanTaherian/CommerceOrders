namespace CommerceOrders.Domain.Exceptions;

public class CartNotFoundException : NotFoundException
{
    public CartNotFoundException(int userId) : base($"User with id {userId} has no cart!")
    {
    }
}
