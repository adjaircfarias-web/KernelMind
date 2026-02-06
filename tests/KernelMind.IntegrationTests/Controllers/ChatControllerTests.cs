using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using KernelMind.Api.DTOs;

namespace KernelMind.IntegrationTests.Controllers;

public class ChatControllerTests : IClassFixture<KernelMindWebApplicationFactory<Program>>, IDisposable
{
    private readonly KernelMindWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ChatControllerTests(KernelMindWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ChatMessage_ReturnsSuccess()
    {
        var request = new ChatRequest { Message = "Hello, I want to order a pizza" };

        var response = await _client.PostAsJsonAsync("/api/chat", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_ChatMessage_WithEmptyMessage_ReturnsBadRequest()
    {
        var request = new ChatRequest { Message = "" };

        var response = await _client.PostAsJsonAsync("/api/chat", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ChatStream_ReturnsStreamingResponse()
    {
        var request = new ChatRequest { Message = "Tell me about your pizzas" };

        var response = await _client.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, "/api/chat/stream")
            {
                Content = JsonContent.Create(request)
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/event-stream");
    }

    [Fact]
    public async Task Post_ChatStreamRaw_ReturnsServerSentEvents()
    {
        var request = new ChatRequest { Message = "What pizzas do you have?" };

        var response = await _client.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, "/api/chat/stream/raw")
            {
                Content = JsonContent.Create(request)
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("text/event-stream");
    }

    [Fact]
    public async Task Post_ChatMessage_WithNullMessage_ReturnsBadRequest()
    {
        var request = new ChatRequest { Message = null };

        var response = await _client.PostAsJsonAsync("/api/chat", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
