using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Discount;

namespace Services.Abstractions
{
    public interface IDiscountService
    {
        Task SetDiscountCodeAsync(DiscountCodeRequestDto discountCodeRequestDto);
    }
}