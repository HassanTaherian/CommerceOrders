namespace CommerceOrders.Domain.Exceptions;

public class EmptySecondCartException : BadRequestException
{
    public EmptySecondCartException(int userId) :
        base($"Second Cart Of User {userId} is Empty!")
    {
    }
}