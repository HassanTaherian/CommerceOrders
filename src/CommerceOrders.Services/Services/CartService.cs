using CommerceOrders.Contracts.UI.Cart;

namespace CommerceOrders.Services.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;

    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = unitOfWork.InvoiceRepository;
    }

    public async Task AddCart(AddProductRequestDto addProductRequestDto, InvoiceState invoiceState)
    {
        var item = MapDtoToInvoiceItem(addProductRequestDto);

        try
        {
            var invoice = _invoiceRepository.GetCartOfUser(addProductRequestDto.UserId);
            await AddItemToInvoice(invoice, item);
        }
        catch (CartNotFoundException)
        {
            await AddItemToNewInvoice(addProductRequestDto.UserId, item);
        }
    }

    private static InvoiceItem MapDtoToInvoiceItem(AddProductRequestDto addProductRequestDto)
    {
        var item = new InvoiceItem
        {
            ProductId = addProductRequestDto.ProductId,
            OriginalPrice = addProductRequestDto.UnitPrice,
            Quantity = addProductRequestDto.Quantity
        };
        return item;
    }

    private async Task AddItemToNewInvoice(int userId, InvoiceItem invoiceItem)
    {
        if (invoiceItem.Quantity <= 0)
        {
            throw new QuantityOutOfRangeInputException();
        }

        var newInvoice = new Invoice
        {
            UserId = userId,
            InvoiceItems = new List<InvoiceItem> { invoiceItem }
        };
        await _invoiceRepository.InsertInvoice(newInvoice);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task AddItemToInvoice(Invoice invoice, InvoiceItem invoiceItem)
    {
        if (invoiceItem.Quantity <= 0)
        {
            throw new QuantityOutOfRangeInputException();
        }

        try
        {
            var existedItem = await _invoiceRepository.GetProductOfInvoice(invoice.Id, invoiceItem.ProductId);
            existedItem.IsDeleted = false;
            existedItem.Quantity = invoiceItem.Quantity;
            existedItem.OriginalPrice = invoiceItem.OriginalPrice;
        }
        catch (InvoiceItemNotFoundException)
        {
            invoice.InvoiceItems.Add(invoiceItem);
        }

        _invoiceRepository.UpdateInvoice(invoice);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCartItemQuantity(UpdateQuantityRequestDto dto)
    {
        if (dto.Quantity <= 0)
        {
            throw new QuantityOutOfRangeInputException();
        }

        var cart = await _invoiceRepository.FetchCartWithSingleItem(dto.UserId, dto.ProductId);

        if (cart is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }

        if (cart.InvoiceItems.Count == 0)
        {
            throw new InvoiceItemNotFoundException(cart.Id, dto.ProductId);
        }

        var item = cart.InvoiceItems.First();

        item.Quantity = dto.Quantity;
        item.IsDeleted = false;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCartItem(DeleteProductRequestDto dto)
    {
        var cart = await _invoiceRepository.FetchCartWithSingleItem(dto.UserId, dto.ProductId);

        if (cart is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }

        if (cart.InvoiceItems.Count == 0)
        {
            throw new InvoiceItemNotFoundException(cart.Id, dto.ProductId);
        }

        cart.InvoiceItems.First().IsDeleted = true;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetCartItems(int userId)
    {
        var cartItems = await _invoiceRepository.FetchCartItems(userId);

        if (cartItems is null)
        {
            throw new CartNotFoundException(userId);
        }

        return MapInvoiceItemToWatchInvoiceItemsResponseDto(cartItems);
    }

    public async Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetDeletedCartItems(int userId)
    {
        var cartItems = await _invoiceRepository.FetchDeletedCartItems(userId);

        if (cartItems is null)
        {
            throw new CartNotFoundException(userId);
        }

        return MapInvoiceItemToWatchInvoiceItemsResponseDto(cartItems);
    }

    private static IEnumerable<WatchInvoiceItemsResponseDto> MapInvoiceItemToWatchInvoiceItemsResponseDto(
    IEnumerable<InvoiceItem> invoiceItems)
    {
        return invoiceItems.Select(item => new WatchInvoiceItemsResponseDto
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.OriginalPrice
        });
    }
}