using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Checkout;
using CommerceOrders.Domain.Exceptions.Order;
using CommerceOrders.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly InvoiceService _invoiceService;
    private readonly IProductAdapter _productAdapter;
    private readonly IMarketingAdapter _marketingAdapter;
    private readonly ICartService _cartService;

    public OrderService(IUnitOfWork uow, InvoiceService invoiceService, IMarketingAdapter marketingAdapter,
        IProductAdapter productAdapter, ICartService cartService)
    {
        _marketingAdapter = marketingAdapter;
        _productAdapter = productAdapter;
        _cartService = cartService;
        _invoiceService = invoiceService;
        _uow = uow;
    }

    public async Task Checkout(CheckoutCommandRequest request)
    {
        Invoice? cart = await _cartService.GetCartWithItems(request.UserId);

        if (cart is null)
        {
            throw new CartNotFoundException(request.UserId);
        }

        cart.Checkout();

        await _productAdapter.UpdateCountingOfProduct(cart!.InvoiceItems, ProductCountingState.ShopState);

        await _uow.SaveChangesAsync();

        await _marketingAdapter.SendInvoiceToMarketing(cart, InvoiceState.Order);
    }


    public async Task<PaginationResultQueryResponse<OrderQueryResponse>> GetOrders(int userId, int page)
    {
        page = page == default ? 1 : page;

        if (page < 1)
        {
            throw new ArgumentException("Page value should be non-negative value");
        }

        List<OrderQueryResponse> orders = await _invoiceService.GetInvoices(userId, InvoiceState.Order)
            .OrderByDescending(order => order.CreatedAt)
            .ThenBy(order => order.Id)
            .Paginate(page)
            .ToOrderQueryResponse()
            .ToListAsync();

        int totalOrders = await _invoiceService.GetInvoices(userId, InvoiceState.Order).CountAsync();

        return orders.ToPaginationResult(totalOrders, page);
    }

    public async Task<OrderWithItemsQueryResponse> GetOrderWithItems(long orderId)
    {
        OrderWithItemsQueryResponse? order = await _uow.Set<Invoice>()
            .Where(o => o.Id == orderId)
            .Include(invoice => invoice.InvoiceItems)
            .ToOrderWithItemsQueryResponse()
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new OrderNotFoundException(orderId);
        }

        return order;
    }
}