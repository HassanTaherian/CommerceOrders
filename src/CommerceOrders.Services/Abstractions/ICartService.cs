using CommerceOrders.Contracts.UI.Cart;

namespace CommerceOrders.Services.Abstractions;

public interface ICartService
{
    Task AddCart(AddProductRequestDto addProductRequestDto, InvoiceState invoiceState);

    Task UpdateQuantity(UpdateQuantityRequestDto updateQuantityRequestDto);

    Task DeleteItem(DeleteProductRequestDto deleteProductRequestDto);

    Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetCartItems(int userId);

    List<WatchInvoiceItemsResponseDto> IsDeletedCartItems(int userId);
}
