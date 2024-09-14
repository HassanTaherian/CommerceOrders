using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Returning;
using CommerceOrders.Contracts.UI.Watch;

namespace Services.Abstractions;

public interface IReturningService
{
    Task Return(ReturningRequestDto dto);
    List<InvoiceResponseDto> ReturnInvoices(int userId);
    Task<IEnumerable<InvoiceItemResponseDto>> ReturnedInvoiceItems(long invoiceId);
}