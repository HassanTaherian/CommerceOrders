namespace CommerceOrders.Services.Exceptions;

public class InvalidPriceRangeException() : BadRequestException("Invalid price range. Start price must be before end price.");