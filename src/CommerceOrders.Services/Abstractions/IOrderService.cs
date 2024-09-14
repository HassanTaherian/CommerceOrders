using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Checkout;
using CommerceOrders.Contracts.UI.Watch;

namespace CommerceOrders.Services.Abstractions
{
    public interface IOrderService
    {
        Task Checkout(CheckoutRequestDto dto);

        List<InvoiceResponseDto> GetAllOrdersOfUser(int userId);

        Task<IEnumerable<InvoiceItemResponseDto>> GetInvoiceItemsOfInvoice(long invoiceId);

    }
}
