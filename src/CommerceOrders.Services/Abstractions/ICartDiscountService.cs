using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Services.Abstractions;

public interface ICartDiscountService
{
    Task Apply(DiscountCodeRequestDto dto);
}