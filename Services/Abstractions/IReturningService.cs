using Contracts.UI;
using Contracts.UI.Invoice;
using Contracts.UI.Order.Returning;
using Contracts.UI.Watch;

namespace Services.Abstractions;

public interface IReturningService
{
    Task Return(ReturningRequestDto dto);
    List<InvoiceResponseDto> ReturnInvoices(int userId);
    Task<IEnumerable<InvoiceItemResponseDto>> ReturnedInvoiceItems(long invoiceId);
}