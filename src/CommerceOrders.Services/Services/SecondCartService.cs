using CommerceOrders.Contracts.UI.SecondCart;

namespace CommerceOrders.Services.Services;

public sealed class SecondCartService : ISecondCartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;

    public SecondCartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = unitOfWork.InvoiceRepository;
    }

    public Invoice GetSecondCart(int userId)
    {
        var secondCart = _invoiceRepository.GetSecondCartOfUser(userId);
        if (secondCart is null)
            throw new EmptySecondCartException(userId);
        return secondCart;
    }
    private async Task<Invoice> CreateSecondCart(int userId)
    {
        var newSecondCart = new Invoice()
        {
            UserId = userId,
            InvoiceItems = new List<InvoiceItem>(),
            State = InvoiceState.SecondCartState
        };
        await _invoiceRepository.InsertInvoice(newSecondCart);
        await _unitOfWork.SaveChangesAsync();
        return newSecondCart;
    }
    public async Task CartToSecondCart(ProductToSecondCartRequestDto productToSecondCardRequestDto)
    {
        var cart = _invoiceRepository.GetCartOfUser(productToSecondCardRequestDto.UserId);
        var cartItem = await _invoiceRepository.GetProductOfInvoice(cart.Id, productToSecondCardRequestDto.ProductId);
        if (cartItem == null)
            throw new InvoiceItemNotFoundException(cart.Id, productToSecondCardRequestDto.ProductId);
        var secondCart = _invoiceRepository.GetSecondCartOfUser
            (productToSecondCardRequestDto.UserId) ?? await CreateSecondCart(productToSecondCardRequestDto.UserId);
        secondCart.InvoiceItems.Add(cartItem);
        cart.InvoiceItems.Remove(cartItem);
        await ApplyChanges(cart, secondCart);
    }
    public async Task SecondCartToCart(ProductToSecondCartRequestDto productToSecondCardRequestDto)
    {
        var cart = _invoiceRepository.GetCartOfUser(productToSecondCardRequestDto.UserId);
        var secondCart = _invoiceRepository.GetSecondCartOfUser
            (productToSecondCardRequestDto.UserId) ?? await CreateSecondCart(productToSecondCardRequestDto.UserId);
        var secondCartItem = secondCart.InvoiceItems
            .FirstOrDefault(item => item.ProductId == productToSecondCardRequestDto.ProductId);
        if (secondCartItem == null)
            throw new InvoiceItemNotFoundException(secondCart.Id, productToSecondCardRequestDto.ProductId);
        cart.InvoiceItems.Add(secondCartItem);
        secondCart.InvoiceItems.Remove(secondCartItem);
        await ApplyChanges(cart, secondCart);
    }
    public async Task DeleteItemFromTheSecondCart(ProductToSecondCartRequestDto productToSecondCartRequestDto)
    {
        var cart = _invoiceRepository.GetCartOfUser(productToSecondCartRequestDto.UserId);
        var secondCart = _invoiceRepository.GetSecondCartOfUser
            (productToSecondCartRequestDto.UserId) ?? await CreateSecondCart(productToSecondCartRequestDto.UserId);
        var secondCartItem = secondCart.InvoiceItems
            .FirstOrDefault(item => item.ProductId == productToSecondCartRequestDto.ProductId);
        if (secondCartItem == null)
            throw new InvoiceItemNotFoundException(secondCart.Id, productToSecondCartRequestDto.ProductId);
        cart.InvoiceItems.Add(secondCartItem);
        cart.InvoiceItems.SingleOrDefault(item => item.ProductId == secondCartItem.ProductId)!.IsDeleted = true;
        secondCart.InvoiceItems.Remove(secondCartItem);
        await ApplyChanges(cart, secondCart);
    }
    private async Task ApplyChanges(Invoice cart, Invoice secondCart)
    {
        _invoiceRepository.UpdateInvoice(cart);
        _invoiceRepository.UpdateInvoice(secondCart);
        await _unitOfWork.SaveChangesAsync();
    }
}