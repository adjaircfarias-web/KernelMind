using KernelMind.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KernelMind.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ChatMessage
/// </summary>
public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("chat_messages", "kernelmind");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Role)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.Content)
            .IsRequired();
        
        builder.Property(e => e.Metadata)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb");
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Relationships
        builder.HasOne(e => e.Session)
            .WithMany(s => s.Messages)
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(e => e.SessionId);
        builder.HasIndex(e => e.Role);
        builder.HasIndex(e => e.CreatedAt);
    }
}
