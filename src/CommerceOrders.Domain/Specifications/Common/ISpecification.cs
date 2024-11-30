namespace CommerceOrders.Domain.Specifications.Common;

public interface ISpecification<in TEntity>
{
    bool IsSatisfied(TEntity entity);
}