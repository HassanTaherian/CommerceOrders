using CommerceOrders.Contracts.Product;
using CommerceOrders.Contracts.UI.Recommendation;
using Domain.Entities;

namespace Services.Abstractions
{
    public interface IRecommendService
    {
        public Task<List<ProductRecommendDto>> Recommended(RecommendationRequestDto recommendationRequestDto);

    }
}