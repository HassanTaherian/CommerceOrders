using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Address;

namespace Services.Abstractions
{
    public interface IAddressService
    {
         Task SetAddressIdAsync(AddressInvoiceDataDto additionalInvoiceDataDto);
    }
}
