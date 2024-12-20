﻿using CommerceOrders.Services.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IReturningService, ReturningService>();
        services.AddScoped<ICartDiscountService, CartDiscountService>();
        services.AddScoped<INextCartService, NextCartService>();
        services.AddScoped<IRecommendService, RecommendService>();
        services.AddScoped<InvoiceService>();
        services.AddSingleton<IHttpProvider, HttpProvider>();
    }
    
    public static void AddExternalServices(this IServiceCollection services)
    {
        services.AddScoped<IProductAdapter, ProductAdapter>();
        services.AddScoped<IMarketingAdapter, MarketingAdapter>();
    }
}