using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Returning;

namespace CommerceOrders.Services.Abstractions;

public interface IReturningService
{
    Task Return(ReturningRequestDto dto);
    List<OrderQueryResponse> ReturnInvoices(int userId);
    Task<IEnumerable<InvoiceItemResponseDto>> ReturnedInvoiceItems(long invoiceId);
}