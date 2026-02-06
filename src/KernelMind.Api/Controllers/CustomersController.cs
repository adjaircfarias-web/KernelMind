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
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll(CancellationToken ct)
    {
        _logger.LogInformation("Getting all customers");
        var customers = await _customerRepository.GetAllAsync(ct);
        return Ok(customers);
    }

    /// <summary>
    /// Gets a specific customer by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Customer>> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting customer: {CustomerId}", id);
        var customer = await _customerRepository.GetByIdAsync(id, ct);
        
        if (customer == null)
            return NotFound();
        
        return Ok(customer);
    }

    /// <summary>
    /// Gets a customer by email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<Customer>> GetByEmail(string email, CancellationToken ct)
    {
        _logger.LogInformation("Getting customer by email: {Email}", email);
        var customer = await _customerRepository.GetByEmailAsync(email, ct);
        
        if (customer == null)
            return NotFound();
        
        return Ok(customer);
    }

    /// <summary>
    /// Gets a customer by phone
    /// </summary>
    [HttpGet("phone/{phone}")]
    public async Task<ActionResult<Customer>> GetByPhone(string phone, CancellationToken ct)
    {
        _logger.LogInformation("Getting customer by phone: {Phone}", phone);
        var customer = await _customerRepository.GetByPhoneAsync(phone, ct);
        
        if (customer == null)
            return NotFound();
        
        return Ok(customer);
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(
        [FromBody] CreateCustomerRequest request, 
        CancellationToken ct)
    {
        _logger.LogInformation("Creating new customer: {Name}", request.Name);
        
        var customer = new Customer
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address
        };
        
        var created = await _customerRepository.CreateAsync(customer, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

public record CreateCustomerRequest(
    string Name,
    string? Phone,
    string? Email,
    string? Address
);
