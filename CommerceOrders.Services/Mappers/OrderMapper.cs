﻿using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Domain.Entities;

namespace CommerceOrders.Services.Mappers;

public class OrderMapper
{
    public List<InvoiceResponseDto> MapInvoicesToInvoiceResponseDtos(IEnumerable<Invoice> invoices)
    {
        return invoices.Select(invoice => new InvoiceResponseDto
            {
                InvoiceId = invoice.Id,
                DateTime = invoice.ShoppingDateTime
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
            UnitPrice = invoiceItem.Price,
            NewPrice = invoiceItem.NewPrice
        });
    }
}