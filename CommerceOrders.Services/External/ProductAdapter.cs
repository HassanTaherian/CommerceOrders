using CommerceOrders.Contracts.Product;
using CommerceOrders.Services.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;

namespace CommerceOrders.Services.External;

public class ProductAdapter : IProductAdapter
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
}