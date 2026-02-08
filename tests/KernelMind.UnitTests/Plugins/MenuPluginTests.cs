using FluentAssertions;
using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using MenuPlugin = KernelMind.Core.Plugins.MenuPlugin;

namespace KernelMind.UnitTests.Plugins;

public class MenuPluginTests
{
    private readonly Mock<IPizzaRepository> _pizzaRepositoryMock;
    private readonly Mock<ILogger<MenuPlugin>> _loggerMock;
    private readonly MenuPlugin _menuPlugin;

    public MenuPluginTests()
    {
        _pizzaRepositoryMock = new Mock<IPizzaRepository>();
        _loggerMock = new Mock<ILogger<MenuPlugin>>();
        _menuPlugin = new MenuPlugin(_pizzaRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetMenuAsync_WithAvailablePizzas_ReturnsMenu()
    {
        var pizzas = new List<Pizza>
        {
            new() { Id = Guid.NewGuid(), Name = "Margherita", Price = 45.00m, Description = "Classic", Category = "Tradicionais", Ingredients = new List<string> { "Tomato", "Mozzarella" } }
        };

        _pizzaRepositoryMock.Setup(x => x.GetAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pizzas);

        var result = await _menuPlugin.GetMenuAsync();

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Margherita");
    }

    [Fact]
    public async Task GetMenuAsync_WithNoPizzas_ReturnsSorryMessage()
    {
        _pizzaRepositoryMock.Setup(x => x.GetAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Pizza>());

        var result = await _menuPlugin.GetMenuAsync();

        result.Should().Contain("Desculpe");
    }

    [Fact]
    public async Task GetPizzaDetailsAsync_WithValidPizza_ReturnsDetails()
    {
        var pizza = new Pizza
        {
            Id = Guid.NewGuid(),
            Name = "Margherita",
            Price = 45.00m,
            Description = "Classic Italian",
            Category = "Tradicionais",
            Ingredients = new List<string> { "Tomato", "Mozzarella", "Basil" },
            IsAvailable = true
        };

        _pizzaRepositoryMock.Setup(x => x.SearchByNameAsync("margherita", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Pizza> { pizza });

        var result = await _menuPlugin.GetPizzaDetailsAsync("margherita");

        result.Should().Contain("Margherita");
        result.Should().Contain("45");
    }

    [Fact]
    public async Task GetPizzaDetailsAsync_WithInvalidPizza_ReturnsNotFound()
    {
        _pizzaRepositoryMock.Setup(x => x.SearchByNameAsync("invalid", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Pizza>());

        var result = await _menuPlugin.GetPizzaDetailsAsync("invalid");

        result.Should().Contain("Não encontrei");
    }

    [Fact]
    public async Task SearchPizzasAsync_WithResults_ReturnsResults()
    {
        var pizzas = new List<Pizza>
        {
            new() { Id = Guid.NewGuid(), Name = "Margherita", Price = 45.00m, Category = "Tradicionais", Ingredients = new List<string>() }
        };

        _pizzaRepositoryMock.Setup(x => x.SearchByNameAsync("tomato", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pizzas);

        var result = await _menuPlugin.SearchPizzasAsync("tomato");

        result.Should().Contain("Resultados");
        result.Should().Contain("Margherita");
    }
}
