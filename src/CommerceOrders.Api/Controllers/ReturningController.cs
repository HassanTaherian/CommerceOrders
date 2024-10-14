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

    [HttpGet]
    [Route("{userId:int}")]
    public async Task<IActionResult> ReturnedOrders([FromRoute] int userId)
    {
        return Ok(await _returningService.GetReturnedOrders(userId));
    }

    [HttpGet]
    [Route("{invoiceId:long}")]
    public async Task<IActionResult> ReturnedOrderItems([FromRoute] long invoiceId)
    {
        var items = await _returningService.ReturnedInvoiceItems(invoiceId);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Return(ReturningRequestDto returningRequestDto)
    {
        await _returningService.Return(returningRequestDto);
        return Ok();
    }
}