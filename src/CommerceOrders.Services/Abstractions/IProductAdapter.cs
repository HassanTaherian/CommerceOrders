namespace CommerceOrders.Services.Abstractions;

public interface IProductAdapter
{
    Task UpdateCountingOfProduct(IEnumerable<InvoiceItem> items, ProductCountingState state);
}