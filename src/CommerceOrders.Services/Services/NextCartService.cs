using CommerceOrders.Contracts.UI.NextCart;

namespace CommerceOrders.Services.Services;

public sealed class NextCartService : INextCartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;

    public NextCartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = unitOfWork.InvoiceRepository;
    }

    public Invoice GetNextCart(int userId)
    {
        var nextCart = _invoiceRepository.GetNextCartOfUser(userId);
        if (nextCart is null)
            throw new EmptyNextCartException(userId);
        return nextCart;
    }

    private async Task<Invoice> CreateNextCart(int userId)
    {
        var nextCart = new Invoice
        {
            UserId = userId,
            InvoiceItems = new List<InvoiceItem>(),
            State = InvoiceState.NextCartState
        };
        _invoiceRepository.Add(nextCart);
        await _unitOfWork.SaveChangesAsync();
        return nextCart;
    }

    public async Task MoveCartItemToNextCart(MoveBetweenNextCartAndCartDto dto)
    {
        var cart = _invoiceRepository.FetchCart(dto.UserId);
        var cartItem = await _invoiceRepository.GetProductOfInvoice(cart.Id, dto.ProductId);
        if (cartItem == null)
            throw new InvoiceItemNotFoundException(cart.Id, dto.ProductId);
        var nextCart = _invoiceRepository.GetNextCartOfUser
            (dto.UserId) ?? await CreateNextCart(dto.UserId);
        nextCart.InvoiceItems.Add(cartItem);
        cart.InvoiceItems.Remove(cartItem);
        await ApplyChanges(cart, nextCart);
    }

    public async Task MoveNextCartItemToCart(MoveBetweenNextCartAndCartDto dto)
    {
        var cart = _invoiceRepository.FetchCart(dto.UserId);
        var nextCart = _invoiceRepository.GetNextCartOfUser
            (dto.UserId) ?? await CreateNextCart(dto.UserId);
        var nextCartItem = nextCart.InvoiceItems
            .FirstOrDefault(item => item.ProductId == dto.ProductId);
        if (nextCartItem == null)
            throw new InvoiceItemNotFoundException(nextCart.Id, dto.ProductId);
        cart.InvoiceItems.Add(nextCartItem);
        nextCart.InvoiceItems.Remove(nextCartItem);
        await ApplyChanges(cart, nextCart);
    }

    public async Task DeleteNextCartItem(MoveBetweenNextCartAndCartDto dto)
    {
        var cart = _invoiceRepository.FetchCart(dto.UserId);
        var nextCart = _invoiceRepository.GetNextCartOfUser
            (dto.UserId) ?? await CreateNextCart(dto.UserId);
        var nextCartItem = nextCart.InvoiceItems
            .FirstOrDefault(item => item.ProductId == dto.ProductId);
        if (nextCartItem == null)
            throw new InvoiceItemNotFoundException(nextCart.Id, dto.ProductId);
        cart.InvoiceItems.Add(nextCartItem);
        cart.InvoiceItems.SingleOrDefault(item => item.ProductId == nextCartItem.ProductId)!.IsDeleted = true;
        nextCart.InvoiceItems.Remove(nextCartItem);
        await ApplyChanges(cart, nextCart);
    }

    private async Task ApplyChanges(Invoice cart, Invoice nextCart)
    {
        _invoiceRepository.UpdateInvoice(cart);
        _invoiceRepository.UpdateInvoice(nextCart);
        await _unitOfWork.SaveChangesAsync();
    }
}