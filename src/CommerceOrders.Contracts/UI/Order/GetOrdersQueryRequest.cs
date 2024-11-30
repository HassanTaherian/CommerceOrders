namespace CommerceOrders.Contracts.UI.Order;

public class GetOrdersQueryRequest
{
    public string? StartDate { get; set; }
    
    public string? EndDate { get; set; }

    public HashSet<int>? Addresses { get; set; }

    public decimal? StartPrice { get; set; }
    
    public decimal? EndPrice { get; set; }
}