namespace CommerceOrders.Domain.Exceptions.Order;

public class InvalidOrderDateException() : BadRequestException("Invalid format for order date.");