using CommerceOrders.Contracts.UI.NextCart;
using CommerceOrders.Domain.Exceptions.SecondCart;

namespace CommerceOrders.Services.Services;

internal sealed class NextCartService : INextCartService
{
    private readonly IUnitOfWork _uow;

    public NextCartService(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task<NextCartResponseDto> GetNextCart(int userId)
    {
        var nextCart = await _uow.InvoiceRepository.FetchNextCart(userId);

        if (nextCart is null)
        {
            throw new NextCartNotFoundException(userId);
        }

        return ToNextCartResponseDto(nextCart);
    }

    private static NextCartResponseDto ToNextCartResponseDto(Invoice secondCart)
    {
        return new NextCartResponseDto
        {
            UserId = secondCart.UserId,
            Items = secondCart.InvoiceItems.Select(item => new NextCartItemResponseDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.OriginalPrice
            }).ToList()
        };
    }

    public async Task MoveCartItemToNextCart(MoveBetweenNextCartAndCartDto dto)
    {
        var cart = _uow.InvoiceRepository.FetchCart(dto.UserId);
        var cartItem = await _uow.InvoiceRepository.GetProductOfInvoice(cart.Id, dto.ProductId);
        if (cartItem == null)
            throw new InvoiceItemNotFoundException(cart.Id, dto.ProductId);
        var nextCart = await _uow.InvoiceRepository.FetchNextCart(dto.UserId) ?? await CreateNextCart(dto.UserId);
        nextCart.InvoiceItems.Add(cartItem);
        cart.InvoiceItems.Remove(cartItem);
        await _uow.SaveChangesAsync();
    }

    public async Task MoveNextCartItemToCart(MoveBetweenNextCartAndCartDto dto)
    {
        var cart = _uow.InvoiceRepository.FetchCart(dto.UserId);
        var nextCart = await _uow.InvoiceRepository.FetchNextCart(dto.UserId) ?? await CreateNextCart(dto.UserId);
        var nextCartItem = nextCart.InvoiceItems
            .FirstOrDefault(item => item.ProductId == dto.ProductId);
        if (nextCartItem == null)
            throw new InvoiceItemNotFoundException(nextCart.Id, dto.ProductId);
        cart.InvoiceItems.Add(nextCartItem);
        nextCart.InvoiceItems.Remove(nextCartItem);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteNextCartItem(MoveBetweenNextCartAndCartDto dto)
    {
        var cart = _uow.InvoiceRepository.FetchCart(dto.UserId);
        var nextCart = await _uow.InvoiceRepository.FetchNextCart(dto.UserId) ?? await CreateNextCart(dto.UserId);
        var nextCartItem = nextCart.InvoiceItems
            .FirstOrDefault(item => item.ProductId == dto.ProductId);
        if (nextCartItem == null)
            throw new InvoiceItemNotFoundException(nextCart.Id, dto.ProductId);
        cart.InvoiceItems.Add(nextCartItem);
        cart.InvoiceItems.SingleOrDefault(item => item.ProductId == nextCartItem.ProductId)!.IsDeleted = true;
        nextCart.InvoiceItems.Remove(nextCartItem);
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
        _uow.InvoiceRepository.Add(nextCart);
        await _uow.SaveChangesAsync();
        return nextCart;
    }
}