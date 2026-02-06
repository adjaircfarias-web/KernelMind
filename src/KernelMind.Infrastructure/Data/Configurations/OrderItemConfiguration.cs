using KernelMind.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KernelMind.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for OrderItem
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items", "kernelmind");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.UnitPrice)
            .HasPrecision(10, 2)
            .IsRequired();
        
        builder.Property(e => e.Quantity)
            .IsRequired();
        
        builder.Property(e => e.Notes)
            .HasMaxLength(500);
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Computed property
        builder.Ignore(e => e.Total);
        
        // Relationships
        builder.HasOne(e => e.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Pizza)
            .WithMany()
            .HasForeignKey(e => e.PizzaId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Indexes
        builder.HasIndex(e => e.OrderId);
        builder.HasIndex(e => e.PizzaId);
    }
}
