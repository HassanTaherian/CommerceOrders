﻿using CommerceOrders.Contracts.Product;
using CommerceOrders.Contracts.UI.Recommendation;
using CommerceOrders.Domain.Entities;

namespace CommerceOrders.Services.Abstractions
{
    public interface IRecommendService
    {
        public Task<List<ProductRecommendDto>> Recommended(RecommendationRequestDto recommendationRequestDto);

    }
}