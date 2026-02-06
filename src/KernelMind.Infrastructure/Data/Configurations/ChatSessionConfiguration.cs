using KernelMind.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KernelMind.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ChatSession
/// </summary>
public class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("chat_sessions", "kernelmind");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.SessionToken)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(e => e.Context)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb");
        
        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(e => e.LastActivityAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Unique index for session token
        builder.HasIndex(e => e.SessionToken).IsUnique();
        
        // Indexes
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.CustomerId);
        builder.HasIndex(e => e.LastActivityAt);
    }
}
