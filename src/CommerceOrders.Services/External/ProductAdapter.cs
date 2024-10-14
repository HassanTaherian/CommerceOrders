using CommerceOrders.Contracts.Product;
using CommerceOrders.Contracts.UI.Recommendation;

namespace CommerceOrders.Services.External;

internal class ProductAdapter : IProductAdapter
{
    private readonly IHttpProvider _httpProvider;

    public ProductAdapter(IHttpProvider httpProvider)
    {
        _httpProvider = httpProvider;
    }

    public async Task UpdateCountingOfProduct(IEnumerable<InvoiceItem> items, ProductCountingState state)
    {
        var countingDtos = MapInvoiceConfig(items, state);
        var jsonBridge = new JsonBridge<ProductUpdateCountingItemRequestDto, Boolean>();
        var json = jsonBridge.SerializeList(countingDtos.ToList());
        await _httpProvider.Post("https://localhost:7083/mock/DiscountMock/UpdateProductCounting", json);
    }

    private ICollection<ProductUpdateCountingItemRequestDto> MapInvoiceConfig(IEnumerable<InvoiceItem> invoiceItems,
        ProductCountingState state)
    {
        var countingDtos = new List<ProductUpdateCountingItemRequestDto>();

        foreach (var invoiceItem in invoiceItems)
        {
            var dto = new ProductUpdateCountingItemRequestDto()
            {
                ProductId = invoiceItem.ProductId,
                ProductCountingState = state,
                Quantity = invoiceItem.Quantity
            };
            countingDtos.Add(dto);
        }

        return countingDtos;
    }

    public async Task<IEnumerable<int>> GetRelatedProducts(int productId)
    {
        ProductRecommendDto mapItem = new()
        {
            ProductId = productId
        };

        JsonBridge<ProductRecommendDto, ProductRecommendDto> jsonBridge =
            new JsonBridge<ProductRecommendDto, ProductRecommendDto>();
        var json = jsonBridge.Serialize(mapItem);
        string response =
            await _httpProvider.Post("https://localhost:7083/mock/DiscountMock/Recommendation", json);


        List<ProductRecommendDto>? result = jsonBridge.DeserializeList(response);

        if (result is null)
        {
            throw new Exception("Network not responding!");
        }

        return result.Select(p => p.ProductId);
    }
}