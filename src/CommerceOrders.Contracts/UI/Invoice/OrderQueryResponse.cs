namespace CommerceOrders.Contracts.UI.Invoice;

public record OrderQueryResponse(long Id, DateTime? DateTime, string? DiscountCode, int Address);
