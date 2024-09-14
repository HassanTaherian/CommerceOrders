namespace CommerceOrders.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public int ProductId { get; set; }

    public double Price { get; set; }

    public double? NewPrice { get; set; }

    public int Quantity { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsReturn { get; set; }
}