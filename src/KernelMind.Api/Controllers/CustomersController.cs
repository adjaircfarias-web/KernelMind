using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KernelMind.Api.Controllers;

/// <summary>
/// API controller for customer management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerRepository customerRepository, 
        ILogger<CustomersController> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all customers
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll(CancellationToken ct)
    {
        _logger.LogInformation("Getting all customers");
        var customers = await _customerRepository.GetAllAsync(ct);
        return Ok(customers.Select(c => CustomerDto.FromEntity(c)));
    }

    /// <summary>
    /// Gets a specific customer by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting customer: {CustomerId}", id);
        var customer = await _customerRepository.GetByIdAsync(id, ct);
        
        if (customer == null)
            return NotFound(new { error = "Customer not found" });
        
        return Ok(CustomerDto.FromEntity(customer));
    }

    /// <summary>
    /// Gets a customer by email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<CustomerDto>> GetByEmail(string email, CancellationToken ct)
    {
        _logger.LogInformation("Getting customer by email: {Email}", email);
        var customer = await _customerRepository.GetByEmailAsync(email, ct);
        
        if (customer == null)
            return NotFound(new { error = "Customer not found" });
        
        return Ok(CustomerDto.FromEntity(customer));
    }

    /// <summary>
    /// Gets a customer by phone
    /// </summary>
    [HttpGet("phone/{phone}")]
    public async Task<ActionResult<CustomerDto>> GetByPhone(string phone, CancellationToken ct)
    {
        _logger.LogInformation("Getting customer by phone: {Phone}", phone);
        var customer = await _customerRepository.GetByPhoneAsync(phone, ct);
        
        if (customer == null)
            return NotFound(new { error = "Customer not found" });
        
        return Ok(CustomerDto.FromEntity(customer));
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(
        [FromBody] CreateCustomerRequest request, 
        CancellationToken ct)
    {
        _logger.LogInformation("Creating new customer: {Name}", request.Name);
        
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { error = "Email is required" });

        var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, ct);
        if (existingCustomer != null)
            return Conflict(new { error = "Customer with this email already exists" });

        var customer = new Customer
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address
        };
        
        var created = await _customerRepository.CreateAsync(customer, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, CustomerDto.FromEntity(created));
    }

    /// <summary>
    /// Updates a customer
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(
        Guid id,
        [FromBody] UpdateCustomerRequest request, 
        CancellationToken ct)
    {
        _logger.LogInformation("Updating customer: {CustomerId}", id);
        
        var customer = await _customerRepository.GetByIdAsync(id, ct);
        if (customer == null)
            return NotFound(new { error = "Customer not found" });

        var updatedCustomer = customer with
        {
            Name = request.Name ?? customer.Name,
            Phone = request.Phone ?? customer.Phone,
            Email = request.Email ?? customer.Email,
            Address = request.Address ?? customer.Address
        };
        
        await _customerRepository.UpdateAsync(updatedCustomer, ct);
        return Ok(CustomerDto.FromEntity(updatedCustomer));
    }
}

/// <summary>
/// Request DTOs
/// </summary>
public record CreateCustomerRequest(
    string Name,
    string? Phone,
    string Email,
    string? Address
);

public record UpdateCustomerRequest(
    string? Name,
    string? Phone,
    string? Email,
    string? Address
);

/// <summary>
/// Response DTOs
/// </summary>
public record CustomerDto(
    Guid Id,
    string Name,
    string? Phone,
    string Email,
    string? Address,
    DateTime CreatedAt
)
{
    public static CustomerDto FromEntity(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.Phone,
        customer.Email ?? "",
        customer.Address,
        customer.CreatedAt
    );
}
