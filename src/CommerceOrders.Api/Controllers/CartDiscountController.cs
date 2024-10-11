using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Api.Controllers;

[ApiController, Route("api/[controller]")]
public class CartDiscountController : ControllerBase
{
    private readonly ICartDiscountService _discountService;

    public CartDiscountController(ICartDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpPatch]
    public async Task<IActionResult> ApplyDiscountCode([FromBody] DiscountCodeRequestDto discountCodeRequestDto)
    {
        await _discountService.Apply(discountCodeRequestDto);
        return Ok();
    }
}