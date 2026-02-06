using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KernelMind.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Customer entity
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == email, ct);
    }

    public async Task<Customer?> GetByPhoneAsync(string phone, CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Phone == phone, ct);
    }

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(ct);
        return customer;
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await _context.Customers.FindAsync(new object[] { id }, ct);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(ct);
        }
    }
}
