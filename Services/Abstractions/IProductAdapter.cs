using Domain.Entities;
using Domain.ValueObjects;

namespace Services.Abstractions;

public interface IProductAdapter
{
    Task UpdateCountingOfProduct(IEnumerable<InvoiceItem> items, ProductCountingState state);
}