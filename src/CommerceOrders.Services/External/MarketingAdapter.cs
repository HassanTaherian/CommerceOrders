using CommerceOrders.Contracts.Marketing.Send;

namespace CommerceOrders.Services.External;

public class MarketingAdapter : IMarketingAdapter
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
}