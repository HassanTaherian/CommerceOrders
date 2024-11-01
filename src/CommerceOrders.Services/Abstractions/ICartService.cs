using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Address;
using CommerceOrders.Contracts.UI.Cart;

namespace CommerceOrders.Services.Abstractions;

public interface ICartService
{
    Task AddCart(AddProductRequestDto addProductRequestDto);

    Task UpdateCartItemQuantity(UpdateQuantityRequestDto dto);

    Task DeleteCartItem(DeleteProductRequestDto deleteProductRequestDto);

    Task<IEnumerable<CartItemQueryResponse>> GetCartItems(int userId);

    Task<IEnumerable<CartItemQueryResponse>> GetDeletedCartItems(int userId);

    Task SetAddress(AddressInvoiceDataDto addressInvoiceDataDto);

    Task<InvoiceItem> GetCartItem(int userId, int productId);

    Task<long> GetCartId(int userId);

    Task<Invoice?> GetCartWithItems(int userId);

    Task<PaginationResultQueryResponse<CartQueryResponse>> GetCarts(int page);
}