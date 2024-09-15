using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Checkout;

namespace CommerceOrders.Services.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IProductAdapter _productAdapter;
    private readonly IMarketingAdapter _marketingAdapter;
    private readonly OrderMapper _orderMapper;

    public OrderService(IUnitOfWork unitOfWork, IMarketingAdapter marketingAdapter, IProductAdapter productAdapter, OrderMapper orderMapper)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.InvoiceRepository;
        _marketingAdapter = marketingAdapter;
        _productAdapter = productAdapter;
        _orderMapper = orderMapper;
    }

    public async Task Checkout(CheckoutRequestDto dto)
    {
        var cart = _invoiceRepository.GetCartOfUser(dto.UserId);
        ValidateCart(cart);

        var notDeletedItems = await _invoiceRepository.GetNotDeleteItems(cart.Id);

        await _productAdapter.UpdateCountingOfProduct(notDeletedItems, ProductCountingState.ShopState);
        await _marketingAdapter.SendInvoiceToMarketing(cart, InvoiceState.OrderState);

        cart.State = InvoiceState.OrderState;
        cart.CreatedAt = DateTime.Now;
        _invoiceRepository.UpdateInvoice(cart);
        await _unitOfWork.SaveChangesAsync();
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

    public List<InvoiceResponseDto> GetAllOrdersOfUser(int userId)
    {
        var invoices = _invoiceRepository.GetInvoiceByState(userId, InvoiceState.OrderState);
        // Todo: Check null in Repository
        if (invoices is null)
        {
            throw new InvoiceNotFoundException(userId);
        }

        return _orderMapper.MapInvoicesToInvoiceResponseDtos(invoices);
    }

    public async Task<IEnumerable<InvoiceItemResponseDto>> GetInvoiceItemsOfInvoice(long invoiceId)
    {
        var invoiceItems = await _invoiceRepository.GetNotDeleteItems(invoiceId);
        // Todo: Check null in Repository
        if (invoiceItems == null)
        {
            throw new EmptyInvoiceException(invoiceId);
        }

        return _orderMapper.MapInvoiceItemsToInvoiceItemResponseDtos(invoiceItems);
    }
}