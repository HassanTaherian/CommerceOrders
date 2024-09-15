using CommerceOrders.Contracts.UI.Order.Checkout;

namespace CommerceOrders.Api.Controllers;

[ApiController, Route("/api/[controller]/[action]")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Route("{userId:int}")]
    public IActionResult GetAllOrdersOfUser(int userId)
    {
        var invoices = _orderService.GetAllOrdersOfUser(userId);
        return Ok(invoices);
    }

    [HttpGet]
    [Route("{invoiceId:long}")]
    public async Task<IActionResult> GetInvoiceItemsOfInvoice(long invoiceId)
    {
        var items = await _orderService.GetInvoiceItemsOfInvoice(invoiceId);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutRequestDto checkout)
    {
        await _orderService.Checkout(checkout);
        return Ok();
    }
}
