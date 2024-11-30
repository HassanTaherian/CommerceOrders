namespace CommerceOrders.Domain.Specifications.Common;

internal class AndSpecification<TEntity>(ISpecification<TEntity> left, ISpecification<TEntity> right)
    : ISpecification<TEntity>
{
    private readonly ISpecification<TEntity> _left = left;
    private readonly ISpecification<TEntity> _right = right;

    public bool IsSatisfied(TEntity entity)
    {
        return _left.IsSatisfied(entity) && _right.IsSatisfied(entity);
    }
}