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
    private readonly IApplicationDbContext _uow;

    public ReturningService(IUnitOfWork unitOfWork, IProductAdapter productAdapter, IMarketingAdapter marketingAdapter,
        InvoiceService invoiceService, IApplicationDbContext uow)
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

        ReturnOrderItems(dto.ProductIds, order);

        IEnumerable<InvoiceItem> returnedItems = order.InvoiceItems.Where(item => item.IsReturned);
        await _productAdapter.UpdateCountingOfProduct(returnedItems, ProductCountingState.ReturnState);

        order.ReturnedAt = DateTime.Now;
        order.State = InvoiceState.Returned;

        await _uow.SaveChangesAsync();
        await _marketingAdapter.SendInvoiceToMarketing(order, InvoiceState.Returned);
    }

    private void ReturnOrderItems(IEnumerable<int> productIds, Invoice order)
    {
        Dictionary<int, InvoiceItem> itemsById = order.InvoiceItems.ToDictionary(item => item.ProductId);

        foreach (int productId in productIds)
        {
            if (itemsById.TryGetValue(productId, out InvoiceItem? invoiceItem))
            {
                throw new OrderItemNotFoundException(order.Id, productId);
            }

            invoiceItem!.IsReturned = true;
        }
    }

    public async Task<OrderWithItemsQueryResponse> GetReturnedOrderWithItems(long orderId)
    {
        OrderWithItemsQueryResponse? order = await _uow.Set<Invoice>()
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