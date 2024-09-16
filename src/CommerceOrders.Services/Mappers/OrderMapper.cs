using CommerceOrders.Contracts.UI.Invoice;

namespace CommerceOrders.Services.Mappers;

public class OrderMapper
{
    public List<InvoiceResponseDto> MapInvoicesToInvoiceResponseDtos(IEnumerable<Invoice> invoices)
    {
        return invoices.Select(invoice => new InvoiceResponseDto
        {
            InvoiceId = invoice.Id,
            DateTime = invoice.CreatedAt
        })
            .ToList();
    }

    public IEnumerable<InvoiceItemResponseDto> MapInvoiceItemsToInvoiceItemResponseDtos(
        IEnumerable<InvoiceItem> invoiceItems)
    {
        return invoiceItems.Select(invoiceItem => new InvoiceItemResponseDto
        {
            ProductId = invoiceItem.ProductId,
            Quantity = invoiceItem.Quantity,
            UnitPrice = invoiceItem.OriginalPrice,
            NewPrice = invoiceItem.NewPrice
        });
    }
}