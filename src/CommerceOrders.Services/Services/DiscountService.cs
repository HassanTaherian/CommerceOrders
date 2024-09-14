using CommerceOrders.Contracts.Discount;
using CommerceOrders.Contracts.UI.Discount;
using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.Exceptions;
using CommerceOrders.Domain.Repositories;
using CommerceOrders.Services.Abstractions;
using CommerceOrders.Services.External;

namespace CommerceOrders.Services.Services;

public sealed class DiscountService : IDiscountService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpProvider _httpProvider;

    public DiscountService(IUnitOfWork unitOfWork, IHttpProvider httpProvider)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.InvoiceRepository;
        _httpProvider = httpProvider;
    }

    public async Task SetDiscountCodeAsync(DiscountCodeRequestDto discountCodeRequestDto)
    {
        var cart = _invoiceRepository.GetCartOfUser(discountCodeRequestDto.UserId);
        var discountResponseDto = await SendDiscountCodeAsync(discountCodeRequestDto);
        if (discountResponseDto is null)
        {
            throw new ExternalModuleException("Discount");
        }
        await ApplyDiscountCode(discountResponseDto, cart.Id);
        cart.DiscountCode = discountCodeRequestDto.DiscountCode;
        _invoiceRepository.UpdateInvoice(cart);
        await _unitOfWork.SaveChangesAsync();
    }
    private async Task<DiscountResponseDto?> SendDiscountCodeAsync(DiscountCodeRequestDto discountCodeRequestDto)
    {
        var discountRequestDto = MapInvoiceToDiscountRequestDto(discountCodeRequestDto);
        var jsonBridge = new JsonBridge<DiscountRequestDto, DiscountResponseDto>();
        var json = jsonBridge.Serialize(discountRequestDto);
        var response = await _httpProvider.Post("https://localhost:7083/mock/DiscountMock/Index", json);
        var discountResponseDto = jsonBridge.Deserialize(response);
        return discountResponseDto;
    }

    private async Task ApplyDiscountCode(DiscountResponseDto discountResponseDto, long invoiceId)
    {
        var invoice = await _invoiceRepository.GetInvoiceById(invoiceId);
        if (invoice is null)
        {
            throw new InvoiceNotFoundException(invoiceId);
        }
        foreach (var discountProductResponseDto in discountResponseDto.Products)
        {
            var items = invoice.InvoiceItems;
            var invoiceItem = items.Single(item => item.ProductId == discountProductResponseDto.ProductId);
            invoiceItem.NewPrice = discountProductResponseDto.UnitPrice;
        }
    }
    private DiscountRequestDto MapInvoiceToDiscountRequestDto(DiscountCodeRequestDto discountCodeRequestDto)
    {
        var invoice = _invoiceRepository.GetCartOfUser(discountCodeRequestDto.UserId);
        var discountRequestDto = new DiscountRequestDto
        {
            DiscountCode = discountCodeRequestDto.DiscountCode,
            UserId = discountCodeRequestDto.UserId,
            TotalPrice = TotalPrice(discountCodeRequestDto.UserId),
            Products = MapInvoiceItemsToDiscountProductRequestDtos(invoice.InvoiceItems)
        };
        return discountRequestDto;
    }

    private IList<DiscountProductRequestDto> MapInvoiceItemsToDiscountProductRequestDtos(IEnumerable<InvoiceItem> invoiceItems)
    {
        return invoiceItems.Where(invoiceItem => invoiceItem.IsDeleted == false)
            .Select(invoiceItem => new DiscountProductRequestDto()
            {
                ProductId = invoiceItem.ProductId,
                Quantity = invoiceItem.Quantity,
                UnitPrice = invoiceItem.Price
            }).ToList();
    }
    private double TotalPrice(int userId)
    {
        var invoice = _invoiceRepository.GetCartOfUser(userId);
        return invoice.InvoiceItems.Where(item => item.IsDeleted == false).Sum(item => item.Price * item.Quantity);
    }
}