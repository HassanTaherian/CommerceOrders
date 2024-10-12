using CommerceOrders.Contracts.UI.Invoice;

namespace CommerceOrders.Services.Mappers;

internal static class OrderMapper
{
    public static List<OrderQueryResponse> MapInvoicesToOrderDtos(IEnumerable<Invoice> invoices)
    {
        return invoices.Select(order =>
                new OrderQueryResponse(order.Id, order.CreatedAt, order.DiscountCode, order.AddressId!.Value))
            .ToList();
    }

    public static IQueryable<OrderQueryResponse> ToOrderQueryResponse(this IQueryable<Invoice> invoices)
    {
        return invoices.Select(order =>
            new OrderQueryResponse(order.Id, order.CreatedAt, order.DiscountCode, order.AddressId!.Value));
    }

    public static IEnumerable<InvoiceItemResponseDto> MapInvoiceItemsToInvoiceItemResponseDtos(
        IEnumerable<InvoiceItem> invoiceItems)
    {
        return invoiceItems.Select(invoiceItem => new InvoiceItemResponseDto
        {
            ProductId = invoiceItem.ProductId,
            Quantity = invoiceItem.Quantity,
            UnitPrice = invoiceItem.OriginalPrice,
            NewPrice = invoiceItem.FinalPrice
        });
    }
}