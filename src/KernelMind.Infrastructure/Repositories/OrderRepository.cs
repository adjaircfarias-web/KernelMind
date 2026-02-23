using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KernelMind.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Order entity
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Pizza)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Pizza)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Pizza)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<Order?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default)
    {
        // Fetch order without includes to avoid tracking conflicts during updates
        // We'll load items separately if needed
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IEnumerable<OrderItem>> GetOrderItemsAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _context.OrderItems
            .AsNoTracking()
            .Where(i => i.OrderId == orderId)
            .ToListAsync(ct);
    }

    public async Task<OrderItem> AddItemToOrderAsync(Guid orderId, OrderItem item, CancellationToken ct = default)
    {
        // Set the OrderId
        item = item with { OrderId = orderId };
        
        // Add item directly to database
        _context.OrderItems.Add(item);
        await _context.SaveChangesAsync(ct);
        
        // Update order total
        var order = await _context.Orders.FindAsync(new object[] { orderId }, ct);
        if (order != null)
        {
            var currentItems = await _context.OrderItems
                .Where(i => i.OrderId == orderId)
                .ToListAsync(ct);
            
            var total = currentItems.Sum(i => i.Quantity * i.UnitPrice);
            order = order with { TotalAmount = total };
            
            // Detach existing tracked order
            var existingEntry = _context.ChangeTracker.Entries<Order>()
                .FirstOrDefault(e => e.Entity.Id == orderId);
            if (existingEntry != null)
            {
                existingEntry.State = EntityState.Detached;
            }
            
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(ct);
        }
        
        return item;
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.Status == status)
            .Include(o => o.Items)
            .ThenInclude(i => i.Pizza)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Order> CreateAsync(Order order, CancellationToken ct = default)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(ct);
        return order;
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        // For records with init-only properties, we need to detach any existing tracked entity first
        var existingEntry = _context.ChangeTracker.Entries<Order>()
            .FirstOrDefault(e => e.Entity.Id == order.Id);
        
        if (existingEntry != null)
        {
            existingEntry.State = EntityState.Detached;
        }
        
        // Also detach any tracked order items for this order
        var existingItems = _context.ChangeTracker.Entries<OrderItem>()
            .Where(e => e.Entity.OrderId == order.Id)
            .ToList();
        
        foreach (var item in existingItems)
        {
            item.State = EntityState.Detached;
        }
        
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _context.Orders.FindAsync(new object[] { id }, ct);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(ct);
        }
    }
}
