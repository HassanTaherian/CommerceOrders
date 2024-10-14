using CommerceOrders.Contracts.Discount;
using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Services.Services;

internal sealed class CartDiscountService : ICartDiscountService
{
    private readonly IUnitOfWork _uow;
    private readonly IMarketingAdapter _marketingAdapter;
    private readonly ICartService _cartService;

    public CartDiscountService(IUnitOfWork uow, IMarketingAdapter marketingAdapter, ICartService cartService)
    {
        _uow = uow;
        _marketingAdapter = marketingAdapter;
        _cartService = cartService;
    }

    public async Task Apply(DiscountCodeRequestDto dto)
    {
        Invoice? cart = await _cartService.GetCartWithItems(dto.UserId);

        if (cart is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }

        DiscountResponseDto? finalPriceResults = await _marketingAdapter.SendDiscountCode(cart, dto);

        if (finalPriceResults is null)
        {
            throw new ExternalModuleException("Discount");
        }

        cart.DiscountCode = dto.DiscountCode;
        ApplyOnCartItemsPrice(cart.InvoiceItems, finalPriceResults);

        await _uow.SaveChangesAsync();
    }
    
    private static void ApplyOnCartItemsPrice(IEnumerable<InvoiceItem> cartItems, DiscountResponseDto dto)
    {
        Dictionary<int, InvoiceItem> items = cartItems.ToDictionary(item => item.ProductId);

        foreach (DiscountProductResponseDto discountItem in dto.Products)
        {
            if (!items.TryGetValue(discountItem.ProductId, out InvoiceItem? item))
            {
                throw new ExternalModuleException("Discount");
            }

            item.FinalPrice = discountItem.UnitPrice;
        }
    }
}