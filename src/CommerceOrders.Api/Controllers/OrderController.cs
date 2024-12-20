﻿using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order;
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

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetOrders([FromRoute] int userId, [FromQuery] int? page,
        [FromQuery] GetOrdersQueryRequest request)
    {
        PaginationResultQueryResponse<OrderQueryResponse> orders = await _orderService.GetOrders(userId, page, request);
        return Ok(orders);
    }

    [HttpGet("{orderId:long}")]
    public async Task<IActionResult> GetOrderWithItems([FromRoute] long orderId)
    {
        OrderWithItemsQueryResponse order = await _orderService.GetOrderWithItems(orderId);
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutCommandRequest request)
    {
        await _orderService.Checkout(request);
        return Ok();
    }
}