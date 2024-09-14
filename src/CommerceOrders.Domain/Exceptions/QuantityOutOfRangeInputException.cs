namespace CommerceOrders.Domain.Exceptions
{
    public sealed class QuantityOutOfRangeInputException : BadRequestException
    {
        public QuantityOutOfRangeInputException()
            : base("Quantity of product must be more than 0")
        {
        }
    }
}