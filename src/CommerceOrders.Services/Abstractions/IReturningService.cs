using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Returning;

namespace CommerceOrders.Services.Abstractions;

public interface IReturningService
{
    Task Return(ReturningRequestDto dto);
    Task<IEnumerable<OrderQueryResponse>> GetReturnedOrders(int userId);
    Task<OrderWithItemsQueryResponse> GetReturnedOrderWithItems(long invoiceId);
}