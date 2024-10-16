﻿using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Checkout;

namespace CommerceOrders.Services.Abstractions;

public interface IOrderService
{
    Task Checkout(CheckoutCommandRequest request);

    Task<IEnumerable<OrderQueryResponse>> GetOrders(int userId);

    Task<OrderWithItemsQueryResponse> GetOrderWithItems(long invoiceId);

}
