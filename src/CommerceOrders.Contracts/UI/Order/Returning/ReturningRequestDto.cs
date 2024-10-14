namespace CommerceOrders.Contracts.UI.Order.Returning;

public class ReturningRequestDto
{
    public long OrderId { get; set; }
    public ICollection<int> ProductIds { get; set; }
}