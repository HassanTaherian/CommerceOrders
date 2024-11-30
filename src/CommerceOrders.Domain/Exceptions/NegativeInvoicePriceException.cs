namespace CommerceOrders.Domain.Exceptions;

public class NegativeInvoicePriceException() : BadRequestException("Price value shouldn't be negative");