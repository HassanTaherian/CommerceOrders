using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.Specifications.Common;

namespace CommerceOrders.Domain.Specifications;

public class PriceLessThanOrEqualSpecification(decimal maxPrice) : ISpecification<Invoice>
{
    public bool IsSatisfied(Invoice order)
    {
        return order.TotalOriginalPrice <= maxPrice;
    }
}