using CommerceOrders.Contracts.UI.Address;

namespace CommerceOrders.Services.Services;

public sealed class AddressService : IAddressService
{
    private readonly IUnitOfWork _uow;

    public AddressService(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task SetAddressIdAsync(AddressInvoiceDataDto addressInvoiceDataDto)
    {
        var invoice = _uow.InvoiceRepository.FetchCart(addressInvoiceDataDto.UserId);

        if (invoice is null)
        {
            throw new InvoiceNotFoundException(addressInvoiceDataDto.UserId);
        }
        invoice.AddressId = addressInvoiceDataDto.AddressId;
        await _uow.SaveChangesAsync();
    }
}