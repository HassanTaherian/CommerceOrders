using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOrders.Persistence.EntityConfigs;

internal class InvoiceItemConfig : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.Property(invoiceItem => invoiceItem.Quantity).HasDefaultValue(1);

        builder.HasQueryFilter(item => !item.IsDeleted);
    }
}