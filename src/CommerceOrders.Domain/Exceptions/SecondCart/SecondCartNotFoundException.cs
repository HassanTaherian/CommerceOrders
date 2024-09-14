namespace CommerceOrders.Domain.Exceptions.SecondCart;

public class SecondCartNotFoundException : NotFoundException
{
    public SecondCartNotFoundException(int userId) : base($"User with id {userId} has no second cart!")
    {
    }
}