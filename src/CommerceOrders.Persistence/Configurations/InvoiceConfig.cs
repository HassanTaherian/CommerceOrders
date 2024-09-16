using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOrders.Persistence.Configurations;

public class InvoiceConfig : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(invoice => invoice.DiscountCode).HasMaxLength(6);
    }
}