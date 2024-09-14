namespace Domain.Exceptions
{
    public class EmptyCartException : BadRequestException
    {
        public EmptyCartException(int userId) : base($"Cart of user {userId} is Empty!")
        {
        }
    }
}
