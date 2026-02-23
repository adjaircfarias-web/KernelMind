using KernelMind.Domain.Entities;
using KernelMind.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace KernelMind.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply PostgreSQL vector extension
        modelBuilder.HasPostgresExtension("vector");

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new PizzaConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ChatSessionConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMessageConfiguration());
    }
}
