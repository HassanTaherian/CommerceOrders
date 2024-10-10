using CommerceOrders.Domain.Repositories;
using CommerceOrders.Persistence.EntityConfigs;

namespace CommerceOrders.Persistence;

public class InvoiceDbContext : DbContext, IApplicationDbContext
{
    public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceConfig).Assembly);
    }

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
}