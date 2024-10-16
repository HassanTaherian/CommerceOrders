using CommerceOrders.Domain.Exceptions;

namespace CommerceOrders.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public int ProductId { get; init; }

    public decimal OriginalPrice { get; set; }

    public decimal? FinalPrice { get; set; }

    private int _quantity;

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
            {
                throw new CartItemQuantityOutOfRangeInputException();
            }

            _quantity = value;
            Restore();
        }
    }


    public bool IsDeleted { get; private set; }

    public void Delete()
    {
        IsDeleted = true;
    }

    public void Restore()
    {
        IsDeleted = false;
    }

    public bool IsReturned { get; set; }

    public long InvoiceId { get; set; }

    public Invoice Invoice { get; set; }
}