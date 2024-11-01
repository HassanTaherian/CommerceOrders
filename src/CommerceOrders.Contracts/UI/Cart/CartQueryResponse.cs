using CommerceOrders.Contracts.UI.Invoice;

namespace CommerceOrders.Contracts.UI.Cart;

public class CartQueryResponse
{
    public int UserId { get; init; }

    public string? DiscountCode { get; set; }

    public int? AddressId { get; set; }
}