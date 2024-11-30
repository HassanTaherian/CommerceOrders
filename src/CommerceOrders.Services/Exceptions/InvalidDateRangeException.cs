namespace CommerceOrders.Services.Exceptions;

public class InvalidDateRangeException() : BadRequestException("Invalid date range. Start date must be before end date.");