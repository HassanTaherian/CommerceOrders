namespace CommerceOrders.Domain.Specifications.Common;

public class TrueSpecification<TEntity> : ISpecification<TEntity>
{
    public bool IsSatisfied(TEntity entity) => true;
}