namespace CommerceOrders.Contracts.UI.Cart;

public class WatchInvoiceItemsResponseDto
{
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
