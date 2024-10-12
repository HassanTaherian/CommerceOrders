namespace CommerceOrders.Domain.ValueObjects;

public enum InvoiceState : byte
{
    Cart,
    NextCart,
    Order,
    Returned
}