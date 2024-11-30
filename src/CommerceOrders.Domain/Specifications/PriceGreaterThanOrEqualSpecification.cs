using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.Specifications.Common;

namespace CommerceOrders.Domain.Specifications;

public class PriceGreaterThanOrEqualSpecification(decimal minPrice) : ISpecification<Invoice>
{
    public bool IsSatisfied(Invoice order)
    {
        return order.TotalFinalPrice >= minPrice;
    }
}