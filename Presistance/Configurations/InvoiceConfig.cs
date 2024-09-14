using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class InvoiceConfig : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(invoice => invoice.UserId).IsRequired();
            builder.Property(invoice => invoice.State).IsRequired();

            builder.Property(invoice => invoice.DiscountCode).HasMaxLength(6);
        }
    }
}