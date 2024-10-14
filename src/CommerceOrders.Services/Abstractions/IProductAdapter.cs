namespace CommerceOrders.Services.Abstractions;

public interface IProductAdapter
{
    Task UpdateCountingOfProduct(IEnumerable<InvoiceItem> items, ProductCountingState state);

    Task<IEnumerable<int>> GetRelatedProducts(int productId);

}