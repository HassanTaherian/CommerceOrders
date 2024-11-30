using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order;
using CommerceOrders.Contracts.UI.Order.Checkout;
using CommerceOrders.Domain.Exceptions.Order;
using CommerceOrders.Domain.Specifications;
using CommerceOrders.Domain.Specifications.Common;
using CommerceOrders.Services.Common;
using CommerceOrders.Services.Exceptions;
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


    public async Task<PaginationResultQueryResponse<OrderQueryResponse>> GetOrders(int userId, int? page,
        GetOrdersQueryRequest request)
    {
        page ??= 1;

        if (page < 1)
        {
            throw new ArgumentException("Page value should be non-negative value");
        }

        ValidateGetOrdersQueryRequest(request);

        ISpecification<Invoice> specification = new TrueSpecification<Invoice>()
            .AndIf(new OrderCreatedOnOrAfterDateSpecification(DateOnly.Parse(request.StartDate!)),
                !string.IsNullOrEmpty(request.StartDate))
            .AndIf(new OrderCreatedOnOrBeforeDateSpecification(DateOnly.Parse(request.EndDate!)),
                !string.IsNullOrEmpty(request.EndDate))
            .AndIf(new AddressInclusionSpecification(request.Addresses!),
                request.Addresses is not null && request.Addresses.Count > 0)
            .AndIf(new PriceGreaterThanOrEqualSpecification(request.StartPrice!.Value), request.StartPrice.HasValue)
            .AndIf(new PriceLessThanOrEqualSpecification(request.EndPrice!.Value), request.EndPrice.HasValue);

        List<OrderQueryResponse> orders = await _invoiceService.GetInvoices(userId, InvoiceState.Order)
            .ApplySpecification(specification)
            .OrderByDescending(order => order.CreatedAt)
            .ThenBy(order => order.Id)
            .Paginate(page.Value)
            .ToOrderQueryResponse()
            .ToListAsync();

        int totalOrders = await _invoiceService.GetInvoices(userId, InvoiceState.Order).CountAsync();

        return orders.ToPaginationResult(totalOrders, page.Value);
    }

    private static void ValidateGetOrdersQueryRequest(GetOrdersQueryRequest request)
    {
        if (!string.IsNullOrEmpty(request.StartDate) && !DateOnly.TryParse(request.StartDate, out _) ||
            !string.IsNullOrEmpty(request.EndDate) && !DateOnly.TryParse(request.EndDate, out _))
        {
            throw new InvalidOrderDateException();
        }

        if (request.StartPrice is <= 0 || request.EndPrice is <= 0)
        {
            throw new NegativeInvoicePriceException();
        }

        if (DateOnly.TryParse(request.StartDate, out var startDate) &&
            DateOnly.TryParse(request.StartDate, out var endDate) &&
            startDate > endDate)
        {
            throw new InvalidDateRangeException();
        }

        if (request.StartPrice > request.EndPrice)
        {
            throw new InvalidPriceRangeException();
        }
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