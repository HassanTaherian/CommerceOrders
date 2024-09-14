using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Domain.Entities;

public class Invoice : BaseEntity
{
    public int UserId { get; set; }

    public InvoiceState State { get; set; }

    public string? DiscountCode { get; set; }

    public DateTime? ShoppingDateTime { get; set; }

    public int? AddressId { get; set; }

    public DateTime? ReturnDateTime { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; set; }
}