﻿using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Returning;
using CommerceOrders.Domain.Exceptions.Returning;

namespace CommerceOrders.Services.Services;

public class ReturningService : IReturningService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IProductAdapter _productAdapter;
    private readonly IMarketingAdapter _marketingAdapter;
    private readonly OrderMapper _orderMapper;

    public ReturningService(IUnitOfWork unitOfWork, IProductAdapter productAdapter, IMarketingAdapter marketingAdapter,
        OrderMapper orderMapper)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.InvoiceRepository;
        _productAdapter = productAdapter;
        _marketingAdapter = marketingAdapter;
        _orderMapper = orderMapper;
    }

    public async Task Return(ReturningRequestDto returningRequestDto)
    {
        var order = await _invoiceRepository.GetInvoiceById(returningRequestDto.InvoiceId);

        switch (order.State)
        {
            case InvoiceState.ReturnState:
                throw new AlreadyReturnedException(returningRequestDto.InvoiceId);
            case InvoiceState.CartState:
                throw new InvoiceIsInCartStateException(returningRequestDto.InvoiceId);
        }

        var invoiceItems = await ChangeStateOfProductItemsToReturned(returningRequestDto.ProductIds, returningRequestDto.InvoiceId);

        await _productAdapter.UpdateCountingOfProduct(invoiceItems, ProductCountingState.ReturnState);
        await _marketingAdapter.SendInvoiceToMarketing(order, InvoiceState.ReturnState);

        order.ReturnedAt = DateTime.Now;
        order.State = InvoiceState.ReturnState;

        _invoiceRepository.UpdateInvoice(order);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<List<InvoiceItem>> ChangeStateOfProductItemsToReturned(IEnumerable<int> productIds, long invoiceId)
    {
        var invoiceItems = new List<InvoiceItem>();

        foreach (var productId in productIds)
        {
            var invoiceItem = await _invoiceRepository.GetProductOfInvoice(invoiceId, productId);

            if (invoiceItem.IsDeleted)
            {
                throw new InvoiceItemNotFoundException(invoiceId, productId);
            }

            invoiceItem.IsReturned = true;
            invoiceItems.Add(invoiceItem);
        }

        return invoiceItems;
    }
    public async Task<IEnumerable<InvoiceItemResponseDto>> ReturnedInvoiceItems(long invoiceId)
    {
        var returnedOrder = await _invoiceRepository.GetInvoiceById(invoiceId);

        if (returnedOrder.InvoiceItems is null)
        {
            throw new EmptyInvoiceException(invoiceId);
        }

        var returnedInvoiceItems = returnedOrder.InvoiceItems.Where(invoiceItem => invoiceItem.IsReturned);
        var invoiceItemResponseDtos = _orderMapper.MapInvoiceItemsToInvoiceItemResponseDtos(returnedInvoiceItems);

        return invoiceItemResponseDtos;
    }

    public List<InvoiceResponseDto> ReturnInvoices(int userId)
    {
        var invoices = _invoiceRepository.GetInvoiceByState(userId, InvoiceState.ReturnState);
        if (invoices == null)
        {
            throw new InvoiceNotFoundException(userId);
        }

        return _orderMapper.MapInvoicesToInvoiceResponseDtos(invoices);
    }
}