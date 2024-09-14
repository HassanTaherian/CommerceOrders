﻿using CommerceOrders.Contracts.UI.Discount;

namespace CommerceOrders.Api.Controllers;

[ApiController, Route("api/[controller]")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountService _discountService;

    public DiscountController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpPatch]
    public async Task<IActionResult> AddDiscountCode([FromBody]
        DiscountCodeRequestDto discountCodeRequestDto)
    {
        await _discountService.SetDiscountCodeAsync
            (discountCodeRequestDto);
        return Ok("Successful");
    }
}