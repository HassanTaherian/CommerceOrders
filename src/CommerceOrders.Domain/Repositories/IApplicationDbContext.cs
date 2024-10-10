using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Domain.Repositories;

public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}