using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Services.Abstractions;

public interface IDiscountService
{
    Task ApplyDiscountCode(DiscountCodeRequestDto discountCodeRequestDto);
}