using CommerceOrders.Contracts.UI.Address;
using CommerceOrders.Contracts.UI.Cart;
using CommerceOrders.Domain.Exceptions.Carts;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class CartService : ICartService
{
    private readonly IApplicationDbContext _uow;

    public CartService(IApplicationDbContext uow, IMarketingAdapter marketingAdapter)
    {
        _uow = uow;
    }

    public async Task AddCart(AddProductRequestDto dto, InvoiceState invoiceState)
    {
        if (dto.Quantity <= 0)
        {
            throw new CartItemQuantityOutOfRangeInputException();
        }

        var cart = await GetCartWithSingleItem(dto.UserId, dto.ProductId);

        if (cart is null)
        {
            CreateCartWithInitialItem(dto.UserId, dto);
        }
        else
        {
            AddItemToExistingCart(cart, dto);
        }

        await _uow.SaveChangesAsync();
    }

    private void CreateCartWithInitialItem(int userId, AddProductRequestDto dto)
    {
        var cart = new Invoice
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

        _uow.Set<Invoice>().Add(cart);
    }

    private static void AddItemToExistingCart(Invoice cart, AddProductRequestDto dto)
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
    }

    public async Task UpdateCartItemQuantity(UpdateQuantityRequestDto dto)
    {
        if (dto.Quantity <= 0)
        {
            throw new CartItemQuantityOutOfRangeInputException();
        }

        InvoiceItem cartItem = await GetCartItem(dto.UserId, dto.ProductId);

        cartItem.Quantity = dto.Quantity;
        cartItem.IsDeleted = false;

        await _uow.SaveChangesAsync();
    }

    public async Task DeleteCartItem(DeleteProductRequestDto dto)
    {
        InvoiceItem cartItem = await GetCartItem(dto.UserId, dto.ProductId);

        cartItem.IsDeleted = true;

        await _uow.SaveChangesAsync();
    }

    private Task<Invoice?> GetCartWithSingleItem(int userId, int productId)
    {
        return _uow.Set<Invoice>()
            .Include(invoice => invoice.InvoiceItems.Where(item => item.ProductId == productId))
            .FirstOrDefaultAsync(invoice => invoice.UserId == userId &&
                                            invoice.State == InvoiceState.CartState);
    }

    public async Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetCartItems(int userId)
    {
        Invoice? cart = await FetchCartWithItems(userId, false)
                .AsNoTracking()
                .FirstOrDefaultAsync()
            ;

        if (cart is null)
        {
            throw new CartNotFoundException(userId);
        }

        return MapInvoiceItemToWatchInvoiceItemsResponseDto(cart.InvoiceItems);
    }

    public async Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetDeletedCartItems(int userId)
    {
        Invoice? cart = await FetchCartWithItems(userId, true)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (cart is null)
        {
            throw new CartNotFoundException(userId);
        }

        return MapInvoiceItemToWatchInvoiceItemsResponseDto(cart.InvoiceItems);
    }

    private IQueryable<Invoice> FetchCartWithItems(int userId, bool isDeleted)
    {
        return _uow.Set<Invoice>()
            .Include(invoice => invoice.InvoiceItems.Where(item => item.IsDeleted == isDeleted))
            .Where(invoice => invoice.UserId == userId &&
                              invoice.State == InvoiceState.CartState);
    }

    public Task<Invoice?> GetCartWithItems(int userId)
    {
        return FetchCartWithItems(userId, false)
            .FirstOrDefaultAsync();
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
        Invoice? cart = await _uow.Set<Invoice>()
            .Where(invoice => invoice.UserId == dto.UserId &&
                              invoice.State == InvoiceState.CartState)
            .FirstOrDefaultAsync();

        if (cart is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }

        cart.AddressId = dto.AddressId;
        await _uow.SaveChangesAsync();
    }

    public async Task<InvoiceItem> GetCartItem(int userId, int productId)
    {
        IQueryable<long> invoiceIds = _uow.Set<Invoice>()
            .Where(i => i.UserId == userId &&
                        i.State == InvoiceState.CartState)
            .Select(i => i.Id);

        InvoiceItem? cartItem = await _uow.Set<InvoiceItem>()
            .Where(item => item.ProductId == productId && invoiceIds.Contains(item.InvoiceId))
            .FirstOrDefaultAsync();

        if (cartItem is null)
        {
            throw new CartItemNotFoundException(userId, productId);
        }

        return cartItem;
    }

    public async Task<long> GetCartId(int userId)
    {
        long id = await _uow.Set<Invoice>()
            .Where(i => i.UserId == userId && i.State == InvoiceState.CartState)
            .Select(i => i.Id)
            .FirstOrDefaultAsync();

        if (id == default)
        {
            throw new CartNotFoundException(userId);
        }

        return id;
    }
}