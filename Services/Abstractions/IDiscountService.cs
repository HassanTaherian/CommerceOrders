using Contracts.UI;
using Contracts.UI.Discount;

namespace Services.Abstractions
{
    public interface IDiscountService
    {
        Task SetDiscountCodeAsync(DiscountCodeRequestDto discountCodeRequestDto);
    }
}