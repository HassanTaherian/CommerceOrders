using CommerceOrders.Contracts.UI.NextCart;
using CommerceOrders.Domain.Exceptions.Carts;
using CommerceOrders.Domain.Exceptions.SecondCart;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal sealed class NextCartService : INextCartService
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICartService _cartService;

    public NextCartService(IUnitOfWork unitOfWork, IApplicationDbContext dbContext, ICartService cartService)
    {
        _uow = unitOfWork;
        _dbContext = dbContext;
        _cartService = cartService;
    }

    public async Task<NextCartResponseDto> GetNextCart(int userId)
    {
        NextCartResponseDto? nextCart = await FetchNextCartWithItems(userId)
            .AsNoTracking()
            .Select(nc => new NextCartResponseDto
            {
                UserId = nc.UserId,
                Items = nc.InvoiceItems.Select(item => new NextCartItemResponseDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.OriginalPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (nextCart is null)
        {
            throw new NextCartNotFoundException(userId);
        }

        return nextCart;
    }

    public async Task MoveCartItemToNextCart(MoveBetweenNextCartAndCartDto dto)
    {
        InvoiceItem? cartItem = await _cartService.GetCartItem(dto.UserId, dto.ProductId);

        if (cartItem is null)
        {
            throw new CartItemNotFoundException(dto.UserId, dto.ProductId);
        }

        Invoice nextCart = await FetchNextCart(dto.UserId)
            .FirstOrDefaultAsync() ?? CreateNextCart(dto.UserId);

        nextCart.InvoiceItems.Add(cartItem);
        await _dbContext.SaveChangesAsync();
    }

    public async Task MoveNextCartItemToCart(MoveBetweenNextCartAndCartDto dto)
    {
        Invoice? cart = _uow.InvoiceRepository.FetchCart(dto.UserId);

        if (cart is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }

        Invoice? nextCart = await FetchNextCartWithSingleItem(dto.UserId, dto.ProductId)
            .FirstOrDefaultAsync();

        if (nextCart is null)
        {
            throw new NextCartNotFoundException(dto.UserId);
        }

        InvoiceItem? nextCartItem = nextCart.InvoiceItems.FirstOrDefault();

        if (nextCartItem is null)
        {
            throw new InvoiceItemNotFoundException(nextCart.Id, dto.ProductId);
        }

        cart.InvoiceItems.Add(nextCartItem);

        await _uow.SaveChangesAsync();
    }

    public async Task DeleteNextCartItem(MoveBetweenNextCartAndCartDto dto)
    {
        Invoice? nextCart = await FetchNextCartWithSingleItem(dto.UserId, dto.ProductId)
            .FirstOrDefaultAsync();

        if (nextCart is null)
        {
            throw new NextCartNotFoundException(dto.UserId);
        }

        InvoiceItem? nextCartItem = nextCart.InvoiceItems.FirstOrDefault();

        if (nextCartItem == null)
        {
            throw new InvoiceItemNotFoundException(nextCart.Id, dto.ProductId);
        }

        nextCartItem.IsDeleted = true;

        await _uow.SaveChangesAsync();
    }

    private Invoice CreateNextCart(int userId)
    {
        var nextCart = new Invoice
        {
            UserId = userId,
            InvoiceItems = new List<InvoiceItem>(),
            State = InvoiceState.NextCartState
        };
        _dbContext.Set<Invoice>()
            .Add(nextCart);
        return nextCart;
    }

    private IQueryable<Invoice> FetchNextCart(int userId)
    {
        return _dbContext.Set<Invoice>()
            .Where(i => i.UserId == userId && i.State == InvoiceState.NextCartState);
    }

    private IQueryable<Invoice> FetchNextCartWithItems(int userId)
    {
        return FetchNextCart(userId)
            .Include(invoice => invoice.InvoiceItems.Where(item => !item.IsDeleted));
    }

    private IQueryable<Invoice> FetchNextCartWithSingleItem(int userId, int productId)
    {
        return FetchNextCart(userId)
            .Include(invoice => invoice.InvoiceItems.Where(item => !item.IsDeleted && item.ProductId == productId));
    }
}