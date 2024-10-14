using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Domain.Repositories;

public interface IUnitOfWork
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}