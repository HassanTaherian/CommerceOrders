using CommerceOrders.Domain.Entities;
using CommerceOrders.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Persistence
{
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
}