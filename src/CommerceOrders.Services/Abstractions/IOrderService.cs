using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Checkout;

namespace CommerceOrders.Services.Abstractions;

public interface IOrderService
{
    Task Checkout(CheckoutRequestDto dto);

    Task<IEnumerable<OrderQueryResponse>> GetOrders(int userId);

    Task<IEnumerable<InvoiceItemResponseDto>> GetInvoiceItemsOfInvoice(long invoiceId);

}
