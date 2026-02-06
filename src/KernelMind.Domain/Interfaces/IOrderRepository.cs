using KernelMind.Domain.Entities;

namespace KernelMind.Domain.Interfaces;

/// <summary>
/// Repository interface for Order entity
/// </summary>
public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default);
    Task<Order> CreateAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
