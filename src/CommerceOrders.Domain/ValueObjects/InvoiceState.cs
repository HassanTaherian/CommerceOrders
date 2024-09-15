namespace CommerceOrders.Domain.ValueObjects;

public enum InvoiceState : byte
{
    CartState,
    SecondCartState,
    OrderState,
    ReturnState
}