using CommerceOrders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Domain.Repositories;

public interface IUnitOfWork
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}