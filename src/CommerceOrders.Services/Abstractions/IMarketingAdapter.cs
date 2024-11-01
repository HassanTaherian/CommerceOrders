using CommerceOrders.Contracts.Discount;
using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Services.Abstractions;

public interface IMarketingAdapter
{
    Task SendInvoiceToMarketing(Invoice invoice, InvoiceState state);

    Task<DiscountResponseDto?> SendDiscountCode(Invoice cart, ApplyCartDiscountCommandRequest dto);

}