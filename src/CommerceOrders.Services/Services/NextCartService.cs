using CommerceOrders.Contracts.UI.NextCart;
using CommerceOrders.Domain.Exceptions.Carts;
using CommerceOrders.Domain.Exceptions.SecondCart;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal sealed class NextCartService : INextCartService
{
    private readonly IApplicationDbContext _uow;
    private readonly ICartService _cartService;

    public NextCartService(IApplicationDbContext unitOfWork, ICartService cartService)
    {
        _uow = unitOfWork;
        _cartService = cartService;
    }

    public async Task<NextCartResponseDto> GetNextCart(int userId)
    {
        NextCartResponseDto? nextCart = await FetchNextCart(userId)
            .AsNoTracking()
            .Include(invoice => invoice.InvoiceItems.Where(item => !item.IsDeleted))
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
        InvoiceItem cartItem = await _cartService.GetCartItem(dto.UserId, dto.ProductId);

        Invoice nextCart = await FetchNextCart(dto.UserId)
            .FirstOrDefaultAsync() ?? CreateNextCart(dto.UserId);

        nextCart.InvoiceItems.Add(cartItem);
        await _uow.SaveChangesAsync();
    }

    public async Task MoveNextCartItemToCart(MoveBetweenNextCartAndCartDto dto)
    {
        InvoiceItem? nextCartItem = await GetNextCartItem(dto.UserId, dto.ProductId);

        if (nextCartItem is null)
        {
            throw new NextCartItemNotFoundException(dto.UserId, dto.ProductId);
        }

        long cartId = await _cartService.GetCartId(dto.UserId);

        nextCartItem.InvoiceId = cartId;

        await _uow.SaveChangesAsync();
    }

    public async Task DeleteNextCartItem(MoveBetweenNextCartAndCartDto dto)
    {
        InvoiceItem? nextCartItem = await GetNextCartItem(dto.UserId, dto.ProductId);

        if (nextCartItem is null)
        {
            throw new NextCartItemNotFoundException(dto.UserId, dto.ProductId);
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
        _uow.Set<Invoice>()
            .Add(nextCart);
        return nextCart;
    }

    private IQueryable<Invoice> FetchNextCart(int userId)
    {
        return _uow.Set<Invoice>()
            .Where(i => i.UserId == userId && i.State == InvoiceState.NextCartState);
    }

    private Task<InvoiceItem?> GetNextCartItem(int userId, int productId)
    {
        IQueryable<long> invoiceIds = FetchNextCart(userId).Select(i => i.Id);

        return _uow.Set<InvoiceItem>()
            .Where(item => item.ProductId == productId && invoiceIds.Contains(item.InvoiceId))
            .FirstOrDefaultAsync();
    }
}