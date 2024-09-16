namespace CommerceOrders.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public int ProductId { get; init; }

    public double OriginalPrice { get; set; }

    public double? FinalPrice { get; set; }

    public int Quantity { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsReturned { get; set; }
}