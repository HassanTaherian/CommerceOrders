namespace CommerceOrders.Contracts.UI;

public class PaginationResultQueryResponse<TItem>
{
    public IEnumerable<TItem> Items { get; set; } = default!;

    public int Page { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}