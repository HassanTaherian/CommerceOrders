namespace CommerceOrders.Contracts.Discount;

public class DiscountProductRequestDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}