namespace Contracts.UI.Invoice;

public class InvoiceItemResponseDto
{
    public int ProductId { get; set; }
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }
    public double? NewPrice { get; set; }
}