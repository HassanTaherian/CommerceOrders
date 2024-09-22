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

    public async Task AddCart(AddProductRequestDto dto, InvoiceState invoiceState)
    {
        if (dto.Quantity <= 0)
        {
            throw new QuantityOutOfRangeInputException();
        }
        
        var cart = await _invoiceRepository.FetchCartWithSingleItem(dto.UserId, dto.ProductId);

        if (cart is null)
        {
            await CreateCartWithInitialItem(dto.UserId, dto);
        }
        else
        {
            await AddItemToExistingCart(cart, dto);
        }
    }

    private async Task CreateCartWithInitialItem(int userId, AddProductRequestDto dto)
    {
        var invoice = new Invoice
        {
            UserId = userId,
            State = InvoiceState.CartState,
            InvoiceItems = new List<InvoiceItem>
            {
                new()
                {
                    ProductId = dto.ProductId,
                    OriginalPrice = dto.UnitPrice,
                    Quantity = dto.Quantity
                }
            }
        };

        _invoiceRepository.Add(invoice);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task AddItemToExistingCart(Invoice cart, AddProductRequestDto dto)
    {
        if (cart.InvoiceItems.Count == 0)
        {
            InvoiceItem item = new()
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                OriginalPrice = dto.UnitPrice
            };
            
            cart.InvoiceItems.Add(item);
        }
        else
        {
            var item = cart.InvoiceItems.First();
            item.IsDeleted = false;
            item.Quantity = dto.Quantity;
            item.OriginalPrice = dto.UnitPrice;
        }

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