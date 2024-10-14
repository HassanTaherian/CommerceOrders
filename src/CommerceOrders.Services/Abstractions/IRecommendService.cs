using CommerceOrders.Contracts.Product;
using CommerceOrders.Contracts.UI.Recommendation;

namespace CommerceOrders.Services.Abstractions;

public interface IRecommendService
{
    public Task<IEnumerable<int>> RecommendProducts(RecommendationRequestDto recommendationRequestDto);
}