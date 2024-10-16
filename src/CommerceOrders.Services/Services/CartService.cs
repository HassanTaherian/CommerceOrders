using CommerceOrders.Contracts.UI.Address;
using CommerceOrders.Contracts.UI.Cart;
using CommerceOrders.Domain.Exceptions.Carts;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class CartService : ICartService
{
    private readonly IUnitOfWork _uow;
    private readonly InvoiceService _invoiceService;

    public CartService(IUnitOfWork uow, InvoiceService invoiceService)
    {
        _uow = uow;
        _invoiceService = invoiceService;
    }

    public async Task AddCart(AddProductRequestDto dto)
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
            State = InvoiceState.Cart,
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
            item.Restore();
            item.Quantity = dto.Quantity;
            item.OriginalPrice = dto.UnitPrice;
        }
    }

    public async Task UpdateCartItemQuantity(UpdateQuantityRequestDto dto)
    {
        InvoiceItem cartItem = await GetCartItem(dto.UserId, dto.ProductId);

        cartItem.Quantity = dto.Quantity;

        await _uow.SaveChangesAsync();
    }

    public async Task DeleteCartItem(DeleteProductRequestDto dto)
    {
        InvoiceItem cartItem = await GetCartItem(dto.UserId, dto.ProductId);

        cartItem.Delete();

        await _uow.SaveChangesAsync();
    }

    private Task<Invoice?> GetCartWithSingleItem(int userId, int productId)
    {
        return QueryCart(userId)
            .Include(invoice => invoice.InvoiceItems.Where(item => item.ProductId == productId && !item.IsDeleted))
            .FirstOrDefaultAsync();
    }

    public Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetCartItems(int userId)
    {
        return GetCartItems(userId, false);
    }

    public Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetDeletedCartItems(int userId)
    {
        return GetCartItems(userId, true);
    }

    private async Task<IEnumerable<WatchInvoiceItemsResponseDto>> GetCartItems(int userId, bool isDeleted)
    {
        ICollection<WatchInvoiceItemsResponseDto> cartItems = await QueryCartItems(userId, isDeleted)
            .Select(item => new WatchInvoiceItemsResponseDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.OriginalPrice
            })
            .ToListAsync();

        if (cartItems is null)
        {
            throw new CartNotFoundException(userId);
        }

        if (cartItems.Count == 0)
        {
            throw new EmptyCartException(userId);
        }

        return cartItems;
    }

    public Task<Invoice?> GetCartWithItems(int userId)
    {
        return QueryCart(userId)
            .Include(invoice => invoice.InvoiceItems.Where(item => !item.IsDeleted))
            .FirstOrDefaultAsync();
    }

    public async Task SetAddress(AddressInvoiceDataDto dto)
    {
        Invoice? cart = await QueryCart(dto.UserId)
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
        InvoiceItem? cartItem = await QueryCartItems(userId, false)
            .Where(item => item.ProductId == productId)
            .FirstOrDefaultAsync();

        if (cartItem is null)
        {
            throw new CartItemNotFoundException(userId, productId);
        }

        return cartItem;
    }

    private IQueryable<InvoiceItem> QueryCartItems(int userId, bool isDeleted)
    {
        IQueryable<long> invoiceIds = QueryCart(userId)
            .Select(i => i.Id);

        return _uow.Set<InvoiceItem>()
            .Where(item => invoiceIds.Contains(item.InvoiceId) && item.IsDeleted == isDeleted);
    }

    public async Task<long> GetCartId(int userId)
    {
        long id = await QueryCart(userId)
            .Select(i => i.Id)
            .FirstOrDefaultAsync();

        if (id == default)
        {
            throw new CartNotFoundException(userId);
        }

        return id;
    }

    private IQueryable<Invoice> QueryCart(int userId)
    {
        return _invoiceService.GetInvoices(userId, InvoiceState.Cart);
    }
}