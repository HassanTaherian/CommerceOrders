﻿// <auto-generated />
using System;
using CommerceOrders.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CommerceOrders.Persistence.Migrations
{
    [DbContext(typeof(InvoiceDbContext))]
    [Migration("20240917205211_ConvertDiscountCodeFromNVarCharToChar")]
    partial class ConvertDiscountCodeFromNVarCharToChar
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CommerceOrders.Domain.Entities.Invoice", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DiscountCode")
                        .HasMaxLength(6)
                        .IsUnicode(false)
                        .HasColumnType("char(6)")
                        .IsFixedLength();

                    b.Property<DateTime?>("ReturnedAt")
                        .HasColumnType("datetime2");

                    b.Property<byte>("State")
                        .HasColumnType("tinyint");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("CommerceOrders.Domain.Entities.InvoiceItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<decimal?>("FinalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long?>("InvoiceId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsReturned")
                        .HasColumnType("bit");

                    b.Property<decimal>("OriginalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.HasKey("Id");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceItems");
                });

            modelBuilder.Entity("CommerceOrders.Domain.Entities.InvoiceItem", b =>
                {
                    b.HasOne("CommerceOrders.Domain.Entities.Invoice", null)
                        .WithMany("InvoiceItems")
                        .HasForeignKey("InvoiceId");
                });

            modelBuilder.Entity("CommerceOrders.Domain.Entities.Invoice", b =>
                {
                    b.Navigation("InvoiceItems");
                });
#pragma warning restore 612, 618
        }
    }
}
