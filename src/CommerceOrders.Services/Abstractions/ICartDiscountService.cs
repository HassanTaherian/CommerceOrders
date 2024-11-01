using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Services.Abstractions;

public interface ICartDiscountService
{
    Task Apply(ApplyCartDiscountCommandRequest request);

    Task Clear(int userId);
}