namespace CommerceOrders.Contracts.UI.Invoice;

public class InvoiceItemResponseDto
{
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal? NewPrice { get; set; }
}