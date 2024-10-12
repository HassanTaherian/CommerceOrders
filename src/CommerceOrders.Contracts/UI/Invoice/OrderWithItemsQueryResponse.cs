namespace CommerceOrders.Contracts.UI.Invoice;

public record OrderWithItemsQueryResponse(
    long OrderId,
    DateTime? DateTime,
    string? DiscountCode,
    int Address,
    IEnumerable<OrderItemQueryResponse> Items);