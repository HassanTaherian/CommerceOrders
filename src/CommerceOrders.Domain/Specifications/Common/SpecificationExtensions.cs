namespace CommerceOrders.Domain.Specifications.Common;

public static class SpecificationExtensions
{
    public static ISpecification<TEntity>
        And<TEntity>(this ISpecification<TEntity> left, ISpecification<TEntity> right) =>
        new AndSpecification<TEntity>(left, right);

    public static ISpecification<TEntity> AndIf<TEntity>(this ISpecification<TEntity> left,
        ISpecification<TEntity> specification, bool condition) => condition ? left.And(specification) : left;

    public static IQueryable<TEntity> ApplySpecification<TEntity>(this IQueryable<TEntity> query,
        ISpecification<TEntity> specification) =>
        query.Where(e => specification.IsSatisfied(e));
}