namespace CommerceOrders.Contracts.UI.Invoice;

public record OrderItemQueryResponse(int ProductId, decimal OriginalPrice, int Quantity, decimal FinalPrice);