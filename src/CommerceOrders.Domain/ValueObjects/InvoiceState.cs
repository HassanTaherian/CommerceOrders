namespace CommerceOrders.Domain.ValueObjects;

public enum InvoiceState : byte
{
    CartState,
    NextCartState,
    OrderState,
    ReturnState
}