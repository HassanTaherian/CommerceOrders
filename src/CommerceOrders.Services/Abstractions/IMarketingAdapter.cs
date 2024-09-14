using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Services.External;

public interface IMarketingAdapter
{
    Task SendInvoiceToMarketing(Invoice invoice, InvoiceState state);
}