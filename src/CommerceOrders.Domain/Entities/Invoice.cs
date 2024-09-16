using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Domain.Entities;

public class Invoice : BaseEntity
{
    public int UserId { get; init; }

    public InvoiceState State { get; set; }

    public string? DiscountCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? AddressId { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; init; } = new List<InvoiceItem>();
}