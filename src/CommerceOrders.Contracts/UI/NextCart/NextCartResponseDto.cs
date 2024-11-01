namespace CommerceOrders.Contracts.UI.NextCart;

public class NextCartResponseDto
{
    public int UserId { get; init; }

    public ICollection<NextCartItemResponseDto> Items { get; init; } = [];
}