using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Address;
using Domain.Exceptions;
using Domain.Repositories;
using Services.Abstractions;

namespace Services.Services
{
    public sealed class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceRepository _invoiceRepository;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = unitOfWork.InvoiceRepository;
        }

        public async Task SetAddressIdAsync(AddressInvoiceDataDto addressInvoiceDataDto)
        {
            var invoice = _invoiceRepository.GetCartOfUser(addressInvoiceDataDto.UserId);
            if (invoice is null)
            {
                throw new InvoiceNotFoundException(addressInvoiceDataDto.UserId);
            }
            invoice.AddressId = addressInvoiceDataDto.AddressId;
            _invoiceRepository.UpdateInvoice(invoice);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}