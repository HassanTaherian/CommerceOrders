using CommerceOrders.Contracts.UI.Address;

namespace CommerceOrders.Services.Abstractions;

public interface IAddressService
{
    Task SetAddressIdAsync(AddressInvoiceDataDto additionalInvoiceDataDto);
}
