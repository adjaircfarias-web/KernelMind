using FluentAssertions;

namespace KernelMind.IntegrationTests.Controllers;

public class HealthControllerTests : IClassFixture<KernelMindWebApplicationFactory<Program>>, IDisposable
{
    private readonly KernelMindWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public HealthControllerTests(KernelMindWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("healthy");
    }

    [Fact]
    public async Task Get_Healthz_ReturnsOk()
    {
        var response = await _client.GetAsync("/healthz");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_Livez_ReturnsOk()
    {
        var response = await _client.GetAsync("/livez");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_Readyz_ReturnsOk()
    {
        var response = await _client.GetAsync("/readyz");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
