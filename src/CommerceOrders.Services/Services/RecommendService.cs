using CommerceOrders.Contracts.UI.Recommendation;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class RecommendService : IRecommendService
{
    private readonly IApplicationDbContext _uow;
    private readonly IProductAdapter _productAdapter;

    public RecommendService(IApplicationDbContext uow, IProductAdapter productAdapter)
    {
        _uow = uow;
        _productAdapter = productAdapter;
    }

    public async Task<IEnumerable<int>> RecommendProducts(RecommendationRequestDto dto)
    {
        IEnumerable<int> relatedProductsFromModule = await _productAdapter.GetRelatedProducts(dto.ProductId);

        List<int> deletedProducts = await GetDeletedProductsInInvoiceItems(dto.UserId, dto.ProductId);

        var mostShoppedProducts = GetMostFrequentShoppedProducts(5);
        
        return relatedProductsFromModule
            .Concat(deletedProducts)
            .Concat(mostShoppedProducts)
            .Distinct();
    }


    private Task<List<int>> GetDeletedProductsInInvoiceItems(int userId, int productId)
    {
        return _uow.Set<InvoiceItem>()
            .Where(item => item.Invoice.UserId == userId)
            .Where(item => item.IsDeleted && item.ProductId != productId)
            .Select(item => item.ProductId)
            .ToListAsync();
    }

    private IEnumerable<int> GetMostFrequentShoppedProducts(int numberOfProducts)
    {
        return
        (
            from item in _uow.Set<InvoiceItem>()
            where !item.IsDeleted && !item.IsReturned
            group item by item.ProductId
            into productGroup
            orderby productGroup.Count() descending
            select productGroup.Key
        ).Take(numberOfProducts);
    }
}