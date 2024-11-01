namespace CommerceOrders.Contracts.UI.Cart;

public class CartItemQueryResponse
{
    public int ProductId { get; set; }

    public decimal OriginalPrice { get; set; }

    public decimal? FinalPrice { get; set; }

    public int Quantity { get; set; }
}