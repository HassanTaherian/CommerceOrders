using Contracts.UI;
using Contracts.UI.Address;

namespace Services.Abstractions
{
    public interface IAddressService
    {
         Task SetAddressIdAsync(AddressInvoiceDataDto additionalInvoiceDataDto);
    }
}
