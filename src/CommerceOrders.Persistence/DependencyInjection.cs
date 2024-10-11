using CommerceOrders.Domain.Repositories;
using CommerceOrders.Persistence;
using CommerceOrders.Persistence.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IApplicationDbContext, InvoiceDbContext>();
    }
}