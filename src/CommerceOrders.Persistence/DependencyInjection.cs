using CommerceOrders.Domain.Repositories;
using CommerceOrders.Persistence;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, InvoiceDbContext>();
    }
}