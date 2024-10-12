namespace CommerceOrders.Contracts.UI.Invoice;

public record OrderQueryResponse(long InvoiceId, DateTime? DateTime, string? DiscountCode, int Address);
