using System.Net.Http.Json;
using FluentAssertions;
using KernelMind.Api.DTOs;

namespace KernelMind.IntegrationTests.Controllers;

public class MenuControllerTests : IClassFixture<KernelMindWebApplicationFactory<Program>>, IDisposable
{
    private readonly KernelMindWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MenuControllerTests(KernelMindWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Menu_ReturnsAllPizzas()
    {
        var response = await _client.GetAsync("/api/menu");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var pizzas = await response.Content.ReadFromJsonAsync<List<PizzaDto>>();
        pizzas.Should().NotBeNull();
        pizzas.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Get_Menu_ReturnsPizzasWithCorrectStructure()
    {
        var response = await _client.GetAsync("/api/menu");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var pizzas = await response.Content.ReadFromJsonAsync<List<PizzaDto>>();
        
        foreach (var pizza in pizzas!)
        {
            pizza.Id.Should().NotBe(Guid.Empty);
            pizza.Name.Should().NotBeNullOrEmpty();
            pizza.Price.Should().BeGreaterThan(0);
            pizza.Category.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task Get_MenuById_ReturnsPizza()
    {
        var response = await _client.GetAsync("/api/menu");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var pizzas = await response.Content.ReadFromJsonAsync<List<PizzaDto>>();
        
        if (pizzas?.Any() == true)
        {
            var firstPizza = pizzas.First();
            var pizzaResponse = await _client.GetAsync($"/api/menu/{firstPizza.Id}");
            
            pizzaResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var pizza = await pizzaResponse.Content.ReadFromJsonAsync<PizzaDto>();
            pizza.Should().NotBeNull();
            pizza!.Id.Should().Be(firstPizza.Id);
        }
    }

    [Fact]
    public async Task Get_MenuById_ReturnsNotFound_ForInvalidId()
    {
        var invalidId = Guid.NewGuid();
        
        var response = await _client.GetAsync($"/api/menu/{invalidId}");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_Categories_ReturnsDistinctCategories()
    {
        var response = await _client.GetAsync("/api/menu/categories");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<string>>();
        categories.Should().NotBeNull();
        categories.Should().HaveCountGreaterThan(0);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
