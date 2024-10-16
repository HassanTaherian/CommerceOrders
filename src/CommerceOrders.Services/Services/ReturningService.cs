using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Returning;
using CommerceOrders.Domain.Exceptions.Order;
using CommerceOrders.Domain.Exceptions.Returning;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class ReturningService : IReturningService
{
    private readonly IProductAdapter _productAdapter;
    private readonly IMarketingAdapter _marketingAdapter;
    private readonly InvoiceService _invoiceService;
    private readonly IUnitOfWork _uow;

    public ReturningService(IProductAdapter productAdapter, IMarketingAdapter marketingAdapter,
        InvoiceService invoiceService, IUnitOfWork uow)
    {
        _productAdapter = productAdapter;
        _marketingAdapter = marketingAdapter;
        _invoiceService = invoiceService;
        _uow = uow;
    }

    public async Task Return(ReturningRequestDto dto)
    {
        Invoice? order = await _uow.Set<Invoice>()
            .Include(i => i.InvoiceItems.Where(item => !item.IsDeleted))
            .Where(i => i.Id == dto.OrderId)
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new OrderNotFoundException(dto.OrderId);
        }

        switch (order.State)
        {
            case InvoiceState.Returned:
                throw new AlreadyReturnedException(dto.OrderId);
            case InvoiceState.NextCart:
            case InvoiceState.Cart:
                throw new OrderNotFoundException(dto.OrderId);
            case InvoiceState.Order:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        order.Return(dto.ProductIds);
        await _productAdapter.UpdateCountingOfProduct(order.ReturnedItems, ProductCountingState.ReturnState);
        
        await _uow.SaveChangesAsync();
        await _marketingAdapter.SendInvoiceToMarketing(order, InvoiceState.Returned);
    }

    public async Task<OrderWithItemsQueryResponse> GetReturnedOrderWithItems(long orderId)
    {
        OrderWithItemsQueryResponse? order = await _uow.Set<Invoice>()
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Include(invoice => invoice.ReturnedItems)
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