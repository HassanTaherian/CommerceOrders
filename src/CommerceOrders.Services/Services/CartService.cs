using CommerceOrders.Contracts.UI.Address;
using CommerceOrders.Contracts.UI.Cart;

namespace CommerceOrders.Services.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _uow;

    public CartService(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task AddCart(AddProductRequestDto dto, InvoiceState invoiceState)
    {
        if (dto.Quantity <= 0)
        {
            throw new CartItemQuantityOutOfRangeInputException();
        }

        var cart = await _uow.InvoiceRepository.FetchCartWithSingleItem(dto.UserId, dto.ProductId);

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

        _uow.InvoiceRepository.Add(invoice);
        await _uow.SaveChangesAsync();
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

        await _uow.SaveChangesAsync();
    }

    public async Task UpdateCartItemQuantity(UpdateQuantityRequestDto dto)
    {
        if (dto.Quantity <= 0)
        {
            throw new CartItemQuantityOutOfRangeInputException();
        }

        var cart = await _uow.InvoiceRepository.FetchCartWithSingleItem(dto.UserId, dto.ProductId);

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

        await _uow.SaveChangesAsync();
    }

    public async Task DeleteCartItem(DeleteProductRequestDto dto)
    {
        var cart = await _uow.InvoiceRepository.FetchCartWithSingleItem(dto.UserId, dto.ProductId);

        if (cart is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }

        if (cart.InvoiceItems.Count == 0)
        {
            throw new InvoiceItemNotFoundException(cart.Id, dto.ProductId);
        }

        cart.InvoiceItems.First().IsDeleted = true;

        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetCartItems(int userId)
    {
        var cartItems = await _uow.InvoiceRepository.FetchCartItems(userId);

        if (cartItems is null)
        {
            throw new CartNotFoundException(userId);
        }

        return MapInvoiceItemToWatchInvoiceItemsResponseDto(cartItems);
    }

    public async Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetDeletedCartItems(int userId)
    {
        var cartItems = await _uow.InvoiceRepository.FetchDeletedCartItems(userId);

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
    
    public async Task SetAddress(AddressInvoiceDataDto dto)
    {
        var invoice = _uow.InvoiceRepository.FetchCart(dto.UserId);

        if (invoice is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }
        invoice.AddressId = dto.AddressId;
        await _uow.SaveChangesAsync();
    }
}