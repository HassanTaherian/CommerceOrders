using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Returning;
using CommerceOrders.Domain.Exceptions.Order;
using CommerceOrders.Domain.Exceptions.Returning;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class ReturningService : IReturningService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IProductAdapter _productAdapter;
    private readonly IMarketingAdapter _marketingAdapter;
    private readonly InvoiceService _invoiceService;
    private readonly IApplicationDbContext _dbContext;

    public ReturningService(IUnitOfWork unitOfWork, IProductAdapter productAdapter, IMarketingAdapter marketingAdapter,
        InvoiceService invoiceService, IApplicationDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.InvoiceRepository;
        _productAdapter = productAdapter;
        _marketingAdapter = marketingAdapter;
        _invoiceService = invoiceService;
        _dbContext = dbContext;
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

        var invoiceItems =
            await ChangeStateOfProductItemsToReturned(returningRequestDto.ProductIds, returningRequestDto.InvoiceId);

        await _productAdapter.UpdateCountingOfProduct(invoiceItems, ProductCountingState.ReturnState);
        await _marketingAdapter.SendInvoiceToMarketing(order, InvoiceState.Returned);

        order.ReturnedAt = DateTime.Now;
        order.State = InvoiceState.Returned;

        _invoiceRepository.UpdateInvoice(order);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<List<InvoiceItem>> ChangeStateOfProductItemsToReturned(IEnumerable<int> productIds,
        long invoiceId)
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

    public async Task<OrderWithItemsQueryResponse> GetReturnedOrderWithItems(long orderId)
    {
        OrderWithItemsQueryResponse? order = await _dbContext.Set<Invoice>()
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Include(invoice => invoice.InvoiceItems.Where(item => item.IsReturned))
            .ToOrderWithItemsQueryResponse()
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new OrderNotFoundException(orderId);
        }

        return order;
    }

    public async Task<IEnumerable<OrderQueryResponse>> GetReturnedOrders(int userId)
    {
        return await _invoiceService.GetInvoices(userId, InvoiceState.Returned)
            .AsNoTracking()
            .ToOrderQueryResponse()
            .ToListAsync();
    }
}