using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.Specifications.Common;

namespace CommerceOrders.Domain.Specifications;

public class AddressInclusionSpecification(HashSet<int> addressIds) : ISpecification<Invoice>
{
    private readonly HashSet<int> _addressIds = addressIds;

    public bool IsSatisfied(Invoice order)
    {
        return _addressIds.Contains(order.AddressId!.Value);
    }
}