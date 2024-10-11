using CommerceOrders.Contracts.Discount;
using CommerceOrders.Contracts.Marketing.Send;
using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Services.External;

internal class MarketingAdapter : IMarketingAdapter
{
    private readonly IHttpProvider _httpProvider;

    public MarketingAdapter(IHttpProvider httpProvider)
    {
        _httpProvider = httpProvider;
    }

    public async Task SendInvoiceToMarketing(Invoice invoice, InvoiceState state)
    {
        var marketingInvoiceRequest = MapInvoiceToMarketingInvoiceRequest(invoice, state);
        var jsonBridge = new JsonBridge<MarketingInvoiceRequest, bool>();
        var json = jsonBridge.Serialize(marketingInvoiceRequest);
        await _httpProvider.Post("https://localhost:7083/mock/DiscountMock/Marketing", json);
    }

    private MarketingInvoiceRequest MapInvoiceToMarketingInvoiceRequest(Invoice invoice, InvoiceState state)
    {
        var marketingInvoiceRequest = new MarketingInvoiceRequest
        {
            InvoiceId = invoice.Id,
            UserId = invoice.UserId,
            InvoiceState = state,
            ShopDateTime = invoice.CreatedAt
        };

        return marketingInvoiceRequest;
    }
    
    public async Task<DiscountResponseDto?> SendDiscountCode(Invoice cart, DiscountCodeRequestDto dto)
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
}