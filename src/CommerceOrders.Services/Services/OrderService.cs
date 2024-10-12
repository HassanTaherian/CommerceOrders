using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Checkout;
using CommerceOrders.Domain.Exceptions.Order;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class OrderService : IOrderService
{
    private readonly IApplicationDbContext _uow;
    private readonly InvoiceService _invoiceService;
    private readonly IProductAdapter _productAdapter;
    private readonly IMarketingAdapter _marketingAdapter;
    private readonly IInvoiceRepository _invoiceRepository;

    public OrderService(IApplicationDbContext uow, InvoiceService invoiceService, IMarketingAdapter marketingAdapter,
        IProductAdapter productAdapter, IInvoiceRepository invoiceRepository)
    {
        _marketingAdapter = marketingAdapter;
        _productAdapter = productAdapter;
        _invoiceRepository = invoiceRepository;
        _invoiceService = invoiceService;
        _uow = uow;
    }

    public async Task Checkout(CheckoutRequestDto dto)
    {
        var cart = _invoiceRepository.FetchCart(dto.UserId);
        ValidateCart(cart);

        var notDeletedItems = await _invoiceRepository.GetNotDeleteItems(cart.Id);

        await _productAdapter.UpdateCountingOfProduct(notDeletedItems, ProductCountingState.ShopState);
        await _marketingAdapter.SendInvoiceToMarketing(cart, InvoiceState.Order);

        cart.State = InvoiceState.Order;
        cart.CreatedAt = DateTime.Now;
        _invoiceRepository.UpdateInvoice(cart);
        await _uow.SaveChangesAsync();
    }

    private void ValidateCart(Invoice cart)
    {
        if (cart.AddressId is null)
        {
            throw new AddressNotSpecifiedException(cart.UserId);
        }

        if (!CartHasItem(cart))
        {
            throw new EmptyCartException(cart.UserId);
        }
    }

    private bool CartHasItem(Invoice cart)
    {
        return cart.InvoiceItems.Any(invoiceItem => invoiceItem.IsDeleted == false);
    }

    public async Task<IEnumerable<OrderQueryResponse>> GetOrders(int userId)
    {
        List<OrderQueryResponse> orders = await _invoiceService.GetInvoices(userId, InvoiceState.Order)
            .AsNoTracking()
            .ToOrderQueryResponse()
            .ToListAsync();

        if (orders.Count == 0)
        {
            return Enumerable.Empty<OrderQueryResponse>();
        }

        return orders;
    }

    public async Task<OrderWithItemsQueryResponse> GetOrderWithItems(long orderId)
    {
        OrderWithItemsQueryResponse? order = await _uow.Set<Invoice>()
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Include(invoice => invoice.InvoiceItems.Where(item => item.IsDeleted == false))
            .ToOrderWithItemsQueryResponse()
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new OrderNotFoundException(orderId);
        }

        return order;
    }
}