using Domain.Entities;
using Domain.ValueObjects;

namespace Services.External;

public interface IMarketingAdapter
{
    Task SendInvoiceToMarketing(Invoice invoice, InvoiceState state);
}