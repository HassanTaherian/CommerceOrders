using CommerceOrders.Contracts.UI.Address;
using CommerceOrders.Contracts.UI.Cart;

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

    [HttpGet]
    public async Task<IActionResult> GetCarts([FromQuery] int? page)
    {
        return Ok(await _cartService.GetCarts(page));
    }

    [HttpGet("/{userId:int}")]
    public async Task<IActionResult> GetCartItems([FromRoute]int userId)
    {
        return Ok(await _cartService.GetCartItems(userId));
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetDeletedCartItems([FromRoute] int userId)
    {
        return Ok(await _cartService.GetDeletedCartItems(userId));
    }

    [HttpPost]
    public Task AddCartItem(AddProductRequestDto addProductRequestDto)
    {
        return _cartService.AddCart(addProductRequestDto);
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

    [HttpPatch]
    public async Task<IActionResult> AddAddress(
        [FromBody] AddressInvoiceDataDto addressInvoiceDataDto)
    {
        await _cartService.SetAddress(addressInvoiceDataDto);
        return Ok();
    }
}