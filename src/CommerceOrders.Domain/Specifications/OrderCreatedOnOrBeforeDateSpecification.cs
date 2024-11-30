using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.Specifications.Common;

namespace CommerceOrders.Domain.Specifications;

public class OrderCreatedOnOrBeforeDateSpecification(DateOnly limit) : ISpecification<Invoice>
{
    private readonly DateOnly _limit = limit;

    public bool IsSatisfied(Invoice entity) => DateOnly.FromDateTime(entity.CreatedAt!.Value) <= _limit;
}