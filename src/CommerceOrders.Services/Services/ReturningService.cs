using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Returning;
using CommerceOrders.Domain.Exceptions.Returning;

namespace CommerceOrders.Services.Services;

internal class ReturningService : IReturningService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IProductAdapter _productAdapter;
    private readonly IMarketingAdapter _marketingAdapter;

    public ReturningService(IUnitOfWork unitOfWork, IProductAdapter productAdapter, IMarketingAdapter marketingAdapter)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.InvoiceRepository;
        _productAdapter = productAdapter;
        _marketingAdapter = marketingAdapter;
    }

    public async Task Return(ReturningRequestDto returningRequestDto)
    {
        var order = await _invoiceRepository.GetInvoiceById(returningRequestDto.InvoiceId);

        switch (order.State)
        {
            case InvoiceState.Returned:
                throw new AlreadyReturnedException(returningRequestDto.InvoiceId);
            case InvoiceState.Cart:
                throw new InvoiceIsInCartStateException(returningRequestDto.InvoiceId);
        }

        var invoiceItems = await ChangeStateOfProductItemsToReturned(returningRequestDto.ProductIds, returningRequestDto.InvoiceId);

        await _productAdapter.UpdateCountingOfProduct(invoiceItems, ProductCountingState.ReturnState);
        await _marketingAdapter.SendInvoiceToMarketing(order, InvoiceState.Returned);

        order.ReturnedAt = DateTime.Now;
        order.State = InvoiceState.Returned;

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
        var invoiceItemResponseDtos = OrderMapper.MapInvoiceItemsToInvoiceItemResponseDtos(returnedInvoiceItems);

        return invoiceItemResponseDtos;
    }

    public List<OrderQueryResponse> ReturnInvoices(int userId)
    {
        var invoices = _invoiceRepository.GetInvoiceByState(userId, InvoiceState.Returned);
        if (invoices == null)
        {
            throw new InvoiceNotFoundException(userId);
        }

        return OrderMapper.MapInvoicesToOrderDtos(invoices);
    }
}