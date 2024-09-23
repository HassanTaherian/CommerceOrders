namespace CommerceOrders.Domain.Exceptions;

public class EmptyNextCartException : BadRequestException
{
    public EmptyNextCartException(int userId) :
        base($"Next Cart Of User {userId} is Empty!")
    {
    }
}