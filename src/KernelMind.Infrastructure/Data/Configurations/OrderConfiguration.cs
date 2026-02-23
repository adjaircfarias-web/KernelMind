using KernelMind.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KernelMind.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Order
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", "kernelmind");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("Id");
        
        builder.Property(e => e.CustomerId)
            .HasColumnName("CustomerId");
        
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("Status");
        
        builder.Property(e => e.TotalAmount)
            .HasPrecision(10, 2)
            .HasDefaultValue(0)
            .HasColumnName("TotalAmount");
        
        builder.Property(e => e.DeliveryAddress)
            .HasMaxLength(500)
            .HasColumnName("DeliveryAddress");
        
        builder.Property(e => e.Notes)
            .HasMaxLength(1000)
            .HasColumnName("Notes");
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("CreatedAt");
        
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("UpdatedAt");
        
        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(e => e.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CustomerId);
        builder.HasIndex(e => e.CreatedAt);
    }
}
