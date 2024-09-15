using CommerceOrders.Contracts.UI.Address;

namespace CommerceOrders.Api.Controllers;

[ApiController, Route("api/[controller]")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpPatch]
    public async Task<IActionResult> AddAddressCode(
        [FromBody] AddressInvoiceDataDto addressInvoiceDataDto)
    {
        await _addressService.SetAddressIdAsync(addressInvoiceDataDto);
        return Ok();
    }
}