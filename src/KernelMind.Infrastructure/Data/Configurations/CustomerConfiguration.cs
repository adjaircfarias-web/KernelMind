using KernelMind.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KernelMind.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Customer
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers", "kernelmind");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Phone)
            .HasMaxLength(20);
        
        builder.Property(e => e.Email)
            .HasMaxLength(200);
        
        builder.Property(e => e.Address)
            .HasMaxLength(500);
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Indexes
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.Phone).IsUnique();
        builder.HasIndex(e => e.Name);
    }
}
