﻿using CommerceOrders.Contracts.UI.Cart;
using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService productService)
    {
        _cartService = productService;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetCartItems(int userId)
    {
        return Ok(await _cartService.GetCartItems(userId));
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetDeletedCartItems(int userId)
    {
        return Ok(await _cartService.GetDeletedCartItems(userId));
    }

    // POST: CartController/Create
    [HttpPost]
    public async Task AddProduct(AddProductRequestDto addProductRequestDto)
    {
        await _cartService.AddCart(addProductRequestDto, InvoiceState.CartState);
    }

    [HttpPatch]
    public Task UpdateCartItemQuantity(UpdateQuantityRequestDto updateQuantityRequestDto)
    {
        return _cartService.UpdateCartItemQuantity(updateQuantityRequestDto);
    }

    [HttpDelete]
    public Task DeleteCartItem(DeleteProductRequestDto deleteProductRequestDto)
    {
        return _cartService.DeleteCartItem(deleteProductRequestDto);
    }
}