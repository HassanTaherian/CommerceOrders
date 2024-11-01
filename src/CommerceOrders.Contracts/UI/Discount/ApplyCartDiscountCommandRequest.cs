namespace CommerceOrders.Contracts.UI.Discount;

public class ApplyCartDiscountCommandRequest
{
    public int UserId { get; set; }

    public string? DiscountCode { get; set; }
}