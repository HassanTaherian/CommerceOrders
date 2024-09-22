using CommerceOrders.Contracts.UI.Cart;
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

    [HttpGet("{id:int}")]
    public IActionResult IsDeletedCartItems(int id)
    {
        var items = _cartService.IsDeletedCartItems(id);
        return Ok(items);
    }


    // POST: CartController/Create
    [HttpPost]
    public async Task AddProduct(AddProductRequestDto addProductRequestDto)
    {
        await _cartService.AddCart(addProductRequestDto, InvoiceState.CartState);
    }

    // PATCH: CartController/Update
    [HttpPatch]
    public async Task UpdateProduct(UpdateQuantityRequestDto updateQuantityRequestDto)
    {
        await _cartService.UpdateQuantity(updateQuantityRequestDto);
    }

    // DELETE: CartController/Delete
    [HttpDelete]
    public async Task DeleteProduct(DeleteProductRequestDto deleteProductRequestDto)
    {
        await _cartService.DeleteItem(deleteProductRequestDto);
    }
}