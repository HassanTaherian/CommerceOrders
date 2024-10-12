using CommerceOrders.Contracts.UI.Invoice;
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
    public async Task<IActionResult> GetOrders(int userId)
    {
        IEnumerable<OrderQueryResponse> orders = await _orderService.GetOrders(userId);
        return Ok(orders);
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