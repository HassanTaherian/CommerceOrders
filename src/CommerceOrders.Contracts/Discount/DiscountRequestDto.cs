namespace CommerceOrders.Contracts.Discount;

public class DiscountRequestDto
{
    public int UserId { get; set; }

    public decimal TotalPrice { get; set; }

    public string? DiscountCode { get; set; }

    public ICollection<DiscountProductRequestDto>? Products { get; set; }
}
