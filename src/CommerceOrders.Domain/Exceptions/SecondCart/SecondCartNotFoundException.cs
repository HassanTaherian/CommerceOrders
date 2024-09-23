namespace CommerceOrders.Domain.Exceptions.SecondCart;

public class NextCartNotFoundException : NotFoundException
{
    public NextCartNotFoundException(int userId) : base($"User with id {userId} has no next cart!")
    {
    }
}