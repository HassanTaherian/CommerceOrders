using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order;
using CommerceOrders.Contracts.UI.Order.Checkout;

namespace CommerceOrders.Services.Abstractions;

public interface IOrderService
{
    Task Checkout(CheckoutCommandRequest request);

    Task<PaginationResultQueryResponse<OrderQueryResponse>> GetOrders(int userId, int? page,
        GetOrdersQueryRequest request);

    Task<OrderWithItemsQueryResponse> GetOrderWithItems(long invoiceId);
}