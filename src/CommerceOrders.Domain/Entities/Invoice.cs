using CommerceOrders.Domain.Exceptions;
using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Domain.Entities;

public class Invoice : BaseEntity
{
    public int UserId { get; init; }

    public InvoiceState State { get; set; }

    public string? DiscountCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? AddressId { get; set; }

    public DateTime? ReturnedAt { get; private set; }

    public ICollection<InvoiceItem> InvoiceItems { get; init; } = [];

    public IEnumerable<InvoiceItem> ReturnedItems => InvoiceItems.Where(item => item.IsReturned);

    public decimal TotalOriginalPrice => InvoiceItems.Sum(item => item.OriginalPrice * item.Quantity);
    
    public decimal TotalFinalPrice => InvoiceItems.Sum(item => item.FinalPrice!.Value * item.Quantity);

    public void Checkout()
    {
        ValidateCheckout();
        State = InvoiceState.Order;
        CreatedAt = DateTime.Now;
    }

    private void ValidateCheckout()
    {
        if (AddressId is null)
        {
            throw new AddressNotSpecifiedException(UserId);
        }

        if (InvoiceItems.All(item => item.IsDeleted))
        {
            throw new EmptyCartException(UserId);
        }
    }

    public void Return(IEnumerable<int> productIds)
    {
        if (State is not InvoiceState.Order)
        {
            throw new InvalidOperationException("Can only return invoice in Order state!");
        }

        ReturnedAt = DateTime.Now;
        State = InvoiceState.Returned;
        ReturnItems(productIds);
    }

    private void ReturnItems(IEnumerable<int> productIds)
    {
        Dictionary<int, InvoiceItem> itemsById = InvoiceItems.ToDictionary(item => item.ProductId);

        foreach (int productId in productIds)
        {
            if (itemsById.TryGetValue(productId, out InvoiceItem? invoiceItem))
            {
                throw new OrderItemNotFoundException(Id, productId);
            }

            invoiceItem!.IsReturned = true;
        }
    }
}