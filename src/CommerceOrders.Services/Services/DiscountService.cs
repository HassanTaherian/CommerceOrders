using CommerceOrders.Contracts.Discount;
using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Services.Services;

public sealed class DiscountService : IDiscountService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpProvider _httpProvider;

    public DiscountService(IUnitOfWork unitOfWork, IHttpProvider httpProvider)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.InvoiceRepository;
        _httpProvider = httpProvider;
    }

    public async Task ApplyDiscountCode(DiscountCodeRequestDto dto)
    {
        var cart = await _invoiceRepository.FetchCartWithItems(dto.UserId);

        if (cart is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }

        var discountResponseDto = await SendDiscountCode(cart, dto);

        if (discountResponseDto is null)
        {
            throw new ExternalModuleException("Discount");
        }

        cart.DiscountCode = dto.DiscountCode;
        ApplyDiscountOnCartItemsPrice(cart.InvoiceItems, discountResponseDto);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<DiscountResponseDto?> SendDiscountCode(Invoice cart, DiscountCodeRequestDto dto)
    {
        var discountRequestDto = CreateDiscountRequestDto(cart, dto);
        var jsonBridge = new JsonBridge<DiscountRequestDto, DiscountResponseDto>();
        var json = jsonBridge.Serialize(discountRequestDto);
        var response = await _httpProvider.Post("https://localhost:7083/mock/DiscountMock/Index", json);
        return jsonBridge.Deserialize(response);
    }
    
    private DiscountRequestDto CreateDiscountRequestDto(Invoice cart, DiscountCodeRequestDto dto)
    {
        return new DiscountRequestDto
        {
            DiscountCode = dto.DiscountCode,
            UserId = dto.UserId,
            TotalPrice = cart.TotalOriginalPrice,
            Products = MapInvoiceItemsToDiscountProductRequestDtos(cart.InvoiceItems)
        };
    }
    
    private IList<DiscountProductRequestDto> MapInvoiceItemsToDiscountProductRequestDtos(
        IEnumerable<InvoiceItem> invoiceItems)
    {
        return invoiceItems.Where(invoiceItem => invoiceItem.IsDeleted == false)
            .Select(invoiceItem => new DiscountProductRequestDto
            {
                ProductId = invoiceItem.ProductId,
                Quantity = invoiceItem.Quantity,
                UnitPrice = invoiceItem.OriginalPrice
            }).ToList();
    }

    private void ApplyDiscountOnCartItemsPrice(IEnumerable<InvoiceItem> cartItems, DiscountResponseDto dto)
    {
        Dictionary<int, InvoiceItem> items = cartItems.ToDictionary(item => item.ProductId);

        foreach (var discountItem in dto.Products)
        {
            if (!items.TryGetValue(discountItem.ProductId, out InvoiceItem? item))
            {
                throw new ExternalModuleException("Discount");
            }

            item.FinalPrice = discountItem.UnitPrice;
        }
    }
}