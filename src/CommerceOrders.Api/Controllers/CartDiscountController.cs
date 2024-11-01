using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Api.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class CartDiscountController : ControllerBase
{
    private readonly ICartDiscountService _discountService;

    public CartDiscountController(ICartDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpPatch]
    public async Task<IActionResult> ApplyDiscountCode(
        [FromBody] ApplyCartDiscountCommandRequest discountCodeRequestDto)
    {
        await _discountService.Apply(discountCodeRequestDto);
        return Ok();
    }

    [HttpPatch("{userId:int}")]
    public async Task<IActionResult> ClearDiscountCode([FromRoute] int userId)
    {
        await _discountService.Clear(userId);
        return Ok();
    }
}