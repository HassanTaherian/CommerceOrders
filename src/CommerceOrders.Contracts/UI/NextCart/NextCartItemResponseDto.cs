namespace CommerceOrders.Contracts.UI.NextCart;

public class NextCartItemResponseDto
{
    public int ProductId { get; init; }

    public decimal Price { get; set; }
    
    public int Quantity { get; set; }
}