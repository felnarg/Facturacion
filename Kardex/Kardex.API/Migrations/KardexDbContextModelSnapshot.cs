using System;
using Kardex.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kardex.API.Migrations
{
    [DbContext(typeof(KardexDbContext))]
    partial class KardexDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.2");

            modelBuilder.Entity("Kardex.Domain.Entities.CreditAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("CreditLimit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("CurrentBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("IdentificationNumber")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("IdentificationType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("PaymentTermDays")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("IdentificationType", "IdentificationNumber")
                        .IsUnique();

                    b.ToTable("CreditAccounts");

                    b.HasData(
                        new
                        {
                            Id = new Guid("b10efb7e-1d4a-4b55-9f45-1f3f9b42a111"),
                            CustomerName = "Comercializadora La 45",
                            IdentificationType = "NIT",
                            IdentificationNumber = "900123456-7",
                            CreditLimit = 5000000m,
                            PaymentTermDays = 30,
                            CurrentBalance = 0m,
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        },
                        new
                        {
                            Id = new Guid("a2ff7e2b-c7f2-4a1b-b0b7-7771e3a2b222"),
                            CustomerName = "Carlos Medina",
                            IdentificationType = "CC",
                            IdentificationNumber = "1030123456",
                            CreditLimit = 1200000m,
                            PaymentTermDays = 20,
                            CurrentBalance = 0m,
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        });
                });

            modelBuilder.Entity("Kardex.Domain.Entities.CreditMovement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("CreditAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("SaleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("CreditAccountId");

                    b.ToTable("CreditMovements");
                });

            modelBuilder.Entity("Kardex.Domain.Entities.CreditMovement", b =>
                {
                    b.HasOne("Kardex.Domain.Entities.CreditAccount", null)
                        .WithMany("Movements")
                        .HasForeignKey("CreditAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
        }
    }
}
