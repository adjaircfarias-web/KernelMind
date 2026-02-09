using KernelMind.Domain.Entities;
using KernelMind.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KernelMind.Infrastructure.Data.Configurations;

public class PizzaConfiguration : IEntityTypeConfiguration<Pizza>
{
    public void Configure(EntityTypeBuilder<Pizza> builder)
    {
        builder.ToTable("pizzas", "kernelmind");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.Description)
            .HasMaxLength(500);
        
        builder.Property(e => e.Price)
            .HasPrecision(10, 2)
            .IsRequired();
        
        builder.Property(e => e.Category)
            .HasMaxLength(50);
        
        builder.Property(e => e.Ingredients);
        
        builder.Property(e => e.IsAvailable)
            .HasDefaultValue(true);
        
        builder.Property(e => e.Embedding)
            .HasColumnType("vector(768)")
            .HasConversion(new VectorValueConverter());
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.HasIndex(e => e.Embedding)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops");
        
        builder.HasIndex(e => e.Category);
        
        builder.HasIndex(e => e.IsAvailable);
    }
}
