using FluentAssertions;
using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using OrderPlugin = KernelMind.Core.Plugins.OrderPlugin;

namespace KernelMind.UnitTests.Plugins;

public class OrderPluginTests
{
    private readonly Mock<IPizzaRepository> _pizzaRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ILogger<OrderPlugin>> _loggerMock;
    private readonly OrderPlugin _orderPlugin;

    public OrderPluginTests()
    {
        _pizzaRepositoryMock = new Mock<IPizzaRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<OrderPlugin>>();
        _orderPlugin = new OrderPlugin(
            _pizzaRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ReturnsOrderConfirmation()
    {
        var result = await _orderPlugin.CreateOrderAsync("John Doe", "123 Main St", "555-1234");

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Pedido Criado");
        result.Should().Contain("John Doe");
    }

    [Fact]
    public async Task AddItemToOrderAsync_WithValidPizza_ReturnsConfirmation()
    {
        var pizza = new Pizza
        {
            Id = Guid.NewGuid(),
            Name = "Margherita",
            Price = 45.00m,
            Category = "Tradicionais",
            Ingredients = new List<string>()
        };

        _pizzaRepositoryMock.Setup(x => x.SearchByNameAsync("margherita", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Pizza> { pizza });

        var createResult = await _orderPlugin.CreateOrderAsync("John", "123 St");
        var orderToken = ExtractOrderToken(createResult);

        var result = await _orderPlugin.AddItemToOrderAsync(orderToken, "margherita", 2);

        result.Should().Contain("Item Adicionado");
        result.Should().Contain("Margherita");
    }

    [Fact]
    public async Task AddItemToOrderAsync_WithInvalidOrder_ReturnsError()
    {
        var result = await _orderPlugin.AddItemToOrderAsync("INVALID", "margherita", 1);

        result.Should().Contain("não encontrado");
    }

    [Fact]
    public void CancelOrder_WithValidOrder_ReturnsCancellation()
    {
        var createResult = _orderPlugin.CreateOrderAsync("John", "123 St").Result;
        var orderToken = ExtractOrderToken(createResult);

        var result = _orderPlugin.CancelOrder(orderToken);

        result.Should().Contain("Cancelado");
    }

    [Fact]
    public void CancelOrder_WithInvalidOrder_ReturnsError()
    {
        var result = _orderPlugin.CancelOrder("INVALID");

        result.Should().Contain("não encontrado");
    }

    private static string ExtractOrderToken(string createResult)
    {
        var match = System.Text.RegularExpressions.Regex.Match(createResult, @"\*\*([A-Z0-9]{8})\*\*");
        return match.Success ? match.Groups[1].Value : "TEST1234";
    }
}
