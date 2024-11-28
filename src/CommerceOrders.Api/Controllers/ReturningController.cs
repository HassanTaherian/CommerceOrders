using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Returning;

namespace CommerceOrders.Api.Controllers;

[ApiController, Route("/api/[controller]/[action]")]
public class ReturningController : Controller
{
    private readonly IReturningService _returningService;

    public ReturningController(IReturningService returningService)
    {
        _returningService = returningService;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetReturnedOrders([FromRoute] int userId) =>
        Ok(await _returningService.GetReturnedOrders(userId));

    [HttpGet("{invoiceId:long}")]
    public async Task<IActionResult> GetReturnedOrderWithItems([FromRoute] long invoiceId) =>
        Ok(await _returningService.GetReturnedOrderWithItems(invoiceId));

    [HttpPost]
    public async Task<IActionResult> Return(ReturningRequestDto returningRequestDto)
    {
        await _returningService.Return(returningRequestDto);
        return Ok();
    }
}