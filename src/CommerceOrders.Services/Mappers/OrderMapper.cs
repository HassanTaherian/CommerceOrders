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

    public static IEnumerable<OrderItemQueryResponse> MapInvoiceItemsToInvoiceItemResponseDtos(
        IEnumerable<InvoiceItem> invoiceItems)
    {
        return invoiceItems.Select(item =>
            new OrderItemQueryResponse(item.ProductId, item.OriginalPrice, item.Quantity, item.FinalPrice!.Value));
    }

    public static IQueryable<OrderWithItemsQueryResponse> ToOrderWithItemsQueryResponse(this IQueryable<Invoice> orders)
    {
        return orders.Select(order =>
            new OrderWithItemsQueryResponse(order.Id, order.CreatedAt, order.DiscountCode, order.AddressId!.Value,
                order.InvoiceItems.Select(item =>
                    new OrderItemQueryResponse(item.ProductId, item.OriginalPrice, item.Quantity,
                        item.FinalPrice!.Value))));
    }
}