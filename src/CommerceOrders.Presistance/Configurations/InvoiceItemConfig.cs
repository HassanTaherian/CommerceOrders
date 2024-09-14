using CommerceOrders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOrders.Persistence.Configurations
{
    public class InvoiceItemConfig : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.Property(invoiceItem => invoiceItem.ProductId).IsRequired();

            builder.Property(invoiceItem => invoiceItem.Quantity).IsRequired();

            builder.Property(invoiceItem => invoiceItem.Quantity).HasDefaultValue(1);
        }
    }
}