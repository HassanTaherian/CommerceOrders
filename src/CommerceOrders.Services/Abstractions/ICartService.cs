using CommerceOrders.Contracts.UI.Cart;

namespace CommerceOrders.Services.Abstractions;

public interface ICartService
{
    Task AddCart(AddProductRequestDto addProductRequestDto, InvoiceState invoiceState);

    Task UpdateCartItemQuantity(UpdateQuantityRequestDto dto);

    Task DeleteCartItem(DeleteProductRequestDto deleteProductRequestDto);

    Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetCartItems(int userId);

    Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetDeletedCartItems(int userId);
}
