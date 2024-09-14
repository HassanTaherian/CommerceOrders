using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Contracts.Product;

public class ProductUpdateCountingItemRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public ProductCountingState ProductCountingState { get; set; }
}
