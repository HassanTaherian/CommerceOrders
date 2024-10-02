using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOrders.Persistence.EntityConfigs;

internal class InvoiceConfig : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(invoice => invoice.DiscountCode)
            .HasMaxLength(6)
            .IsFixedLength()
            .IsUnicode(false);
    }
}