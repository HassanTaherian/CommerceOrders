﻿using CommerceOrders.Contracts.UI.NextCart;
using CommerceOrders.Domain.Entities;

namespace CommerceOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class NextCartController : ControllerBase
{
    private readonly INextCartService _nextCartService;

    public NextCartController(INextCartService nextCartService)
    {
        _nextCartService = nextCartService;
    }

    [HttpGet("{userId:int}")]
    public IEnumerable<InvoiceItem> GetNextCart(int userId)
    {
        return _nextCartService.GetNextCart(userId).InvoiceItems;
    }

    [HttpPatch]
    public async Task<IActionResult> MoveCartItemToNextCart([FromBody] MoveBetweenNextCartAndCartDto productToSecondCartRequestDto)
    {
        await _nextCartService.MoveCartItemToNextCart(productToSecondCartRequestDto);
        return Ok("Successful");
    }

    [HttpPatch]
    public async Task<IActionResult> MoveNextCartItemToCart([FromBody] MoveBetweenNextCartAndCartDto productToSecondCartRequestDto)
    {
        await _nextCartService.MoveNextCartItemToCart(productToSecondCartRequestDto);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteNextCartItem([FromBody] MoveBetweenNextCartAndCartDto productToSecondCartRequestDto)
    {
        await _nextCartService.DeleteNextCartItem(productToSecondCartRequestDto);
        return Ok();
    }
}