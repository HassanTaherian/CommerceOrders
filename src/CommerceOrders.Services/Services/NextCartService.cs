using CommerceOrders.Contracts.UI.NextCart;
using CommerceOrders.Domain.Exceptions.SecondCart;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal sealed class NextCartService : INextCartService
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _dbContext;

    public NextCartService(IUnitOfWork unitOfWork, IApplicationDbContext dbContext)
    {
        _uow = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<NextCartResponseDto> GetNextCart(int userId)
    {
        NextCartResponseDto? nextCart = await FetchNextCart(userId)
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
        // Todo: Should use ICartService
        // Todo: Should only fetch cart and single item
        Invoice? cart = await _uow.InvoiceRepository.FetchCartWithItems(dto.UserId);

        if (cart is null)
        {
            throw new CartNotFoundException(dto.UserId);
        }

        var cartItem = cart.InvoiceItems.FirstOrDefault();

        if (cartItem is null)
        {
            throw new InvoiceItemNotFoundException(cart.Id, dto.ProductId);
        }

        // Todo: should not fetch cart items
        Invoice nextCart = await FetchNextCart(dto.UserId)
            .FirstOrDefaultAsync() ?? await CreateNextCart(dto.UserId);
        nextCart.InvoiceItems.Add(cartItem);
        await _uow.SaveChangesAsync();
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

    private async Task<Invoice> CreateNextCart(int userId)
    {
        var nextCart = new Invoice
        {
            UserId = userId,
            InvoiceItems = new List<InvoiceItem>(),
            State = InvoiceState.NextCartState
        };
        _dbContext.Set<Invoice>().Add(nextCart);
        // Todo: Unnecessary SaveChanges
        await _dbContext.SaveChangesAsync();
        return nextCart;
    }

    private IQueryable<Invoice> FetchNextCart(int userId)
    {
        return _dbContext.Set<Invoice>()
            .Include(invoice => invoice.InvoiceItems.Where(item => !item.IsDeleted))
            .Where(invoice => invoice.UserId == userId &&
                              invoice.State == InvoiceState.NextCartState);
    }

    private IQueryable<Invoice> FetchNextCartWithSingleItem(int userId, int productId)
    {
        return _dbContext.Set<Invoice>()
            .Include(invoice => invoice.InvoiceItems.Where(item => !item.IsDeleted && item.ProductId == productId))
            .Where(invoice => invoice.UserId == userId &&
                              invoice.State == InvoiceState.NextCartState);
    }
}