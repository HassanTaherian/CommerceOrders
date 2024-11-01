using CommerceOrders.Contracts.UI;
using CommerceOrders.Contracts.UI.Address;
using CommerceOrders.Contracts.UI.Cart;
using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Domain.Exceptions.Carts;
using CommerceOrders.Services.Common;
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
            .Include(invoice => invoice.InvoiceItems.Where(item => item.ProductId == productId))
            .FirstOrDefaultAsync();
    }

    public Task<IEnumerable<CartItemQueryResponse>> GetCartItems(int userId)
    {
        return GetCartItems(userId, false);
    }

    public Task<IEnumerable<CartItemQueryResponse>> GetDeletedCartItems(int userId)
    {
        return GetCartItems(userId, true);
    }

    private async Task<IEnumerable<CartItemQueryResponse>> GetCartItems(int userId, bool isDeleted)
    {
        IQueryable<InvoiceItem> query = QueryCartItems(userId);

        if (isDeleted)
        {
            query = query.IgnoreQueryFilters()
                .Where(item => item.IsDeleted);
        }

        ICollection<CartItemQueryResponse> cartItems = await query.Select(item =>
                new CartItemQueryResponse
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    OriginalPrice = item.OriginalPrice,
                    FinalPrice = item.FinalPrice
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
            .Include(invoice => invoice.InvoiceItems)
            .FirstOrDefaultAsync();
    }

    public async Task<PaginationResultQueryResponse<CartQueryResponse>> GetCarts(int? page)
    {
        page ??= 1;

        if (page < 1)
        {
            throw new ArgumentException("Page value should be non-negative value");
        }

        List<CartQueryResponse> cartQueryResponses = await _uow.Set<Invoice>()
            .Where(i => i.State == InvoiceState.Cart)
            .OrderBy(c => c.Id)
            .Paginate(page.Value)
            .Select(c => new CartQueryResponse
            {
                UserId = c.UserId,
                AddressId = c.AddressId,
                DiscountCode = c.DiscountCode
            })
            .ToListAsync();

        int totalCarts = await _uow.Set<Invoice>()
            .Where(c => c.State == InvoiceState.Cart)
            .CountAsync();

        return cartQueryResponses.ToPaginationResult(totalCarts, page.Value);
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
        InvoiceItem? cartItem = await QueryCartItems(userId)
            .Where(item => item.ProductId == productId)
            .FirstOrDefaultAsync();

        if (cartItem is null)
        {
            throw new CartItemNotFoundException(userId, productId);
        }

        return cartItem;
    }

    private IQueryable<InvoiceItem> QueryCartItems(int userId)
    {
        IQueryable<long> invoiceIds = QueryCart(userId)
            .Select(i => i.Id);

        return _uow.Set<InvoiceItem>()
            .Where(item => invoiceIds.Contains(item.InvoiceId));
    }

    private IQueryable<InvoiceItem> QueryDeletedCartItems(int userId)
    {
        return QueryCartItems(userId).IgnoreQueryFilters();
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