using CommerceOrders.Persistence.Configurations;

namespace CommerceOrders.Persistence;

public class InvoiceContext : DbContext
{
    public InvoiceContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InvoiceConfig());
        modelBuilder.ApplyConfiguration(new InvoiceItemConfig());
    }

    public virtual DbSet<Invoice> Invoices { get; set; }
    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }
}