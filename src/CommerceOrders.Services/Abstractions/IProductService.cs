﻿using CommerceOrders.Contracts.UI.Cart;

namespace CommerceOrders.Services.Abstractions;

public interface IProductService
{
    Task AddCart(AddProductRequestDto addProductRequestDto, InvoiceState invoiceState);

    Task UpdateQuantity(UpdateQuantityRequestDto updateQuantityRequestDto);

    Task DeleteItem(DeleteProductRequestDto deleteProductRequestDto);

    List<WatchInvoiceItemsResponseDto> ExistedCartItems(int userId);

    List<WatchInvoiceItemsResponseDto> IsDeletedCartItems(int userId);
}
