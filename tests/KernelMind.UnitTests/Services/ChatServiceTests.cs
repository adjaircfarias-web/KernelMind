using FluentAssertions;
using KernelMind.Core.Services;
using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace KernelMind.UnitTests.Services;

public class ChatServiceTests
{
    private readonly Mock<IChatClient> _chatClientMock;
    private readonly Mock<ILogger<ChatService>> _loggerMock;
    private readonly Mock<KernelMind.Core.Plugins.MenuPlugin> _menuPluginMock;
    private readonly Mock<KernelMind.Core.Plugins.OrderPlugin> _orderPluginMock;
    private readonly Mock<KernelMind.Core.Plugins.CalculationPlugin> _calculationPluginMock;
    private readonly Mock<KernelMind.Core.Plugins.ContextPlugin> _contextPluginMock;
    private readonly Mock<IChatSessionRepository> _chatSessionRepositoryMock;
    private readonly ChatService _chatService;

    public ChatServiceTests()
    {
        _chatClientMock = new Mock<IChatClient>();
        _loggerMock = new Mock<ILogger<ChatService>>();
        _menuPluginMock = new Mock<KernelMind.Core.Plugins.MenuPlugin>(
            new Mock<IPizzaRepository>().Object,
            Mock.Of<ILogger<KernelMind.Core.Plugins.MenuPlugin>>());
        _orderPluginMock = new Mock<KernelMind.Core.Plugins.OrderPlugin>(
            new Mock<IPizzaRepository>().Object,
            new Mock<IOrderRepository>().Object,
            Mock.Of<ILogger<KernelMind.Core.Plugins.OrderPlugin>>());
        _calculationPluginMock = new Mock<KernelMind.Core.Plugins.CalculationPlugin>(
            Mock.Of<ILogger<KernelMind.Core.Plugins.CalculationPlugin>>());
        _contextPluginMock = new Mock<KernelMind.Core.Plugins.ContextPlugin>(Mock.Of<ILogger<KernelMind.Core.Plugins.ContextPlugin>>());
        _chatSessionRepositoryMock = new Mock<IChatSessionRepository>();

        _chatService = new ChatService(
            _chatClientMock.Object,
            _loggerMock.Object,
            _menuPluginMock.Object,
            _orderPluginMock.Object,
            _calculationPluginMock.Object,
            _contextPluginMock.Object,
            _chatSessionRepositoryMock.Object);
    }

    [Fact]
    public void ChatClient_ShouldReturnChatClient()
    {
        var result = _chatService.ChatClient;

        result.Should().Be(_chatClientMock.Object);
    }

    [Fact]
    public async Task ProcessMessageAsync_WithEmptyMessage_ThrowsException()
    {
        var sessionId = "test-session";
        var message = "";

        Func<Task> act = () => _chatService.ProcessMessageAsync(sessionId, message, CancellationToken.None);

        act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ProcessMessageAsync_WithNullMessage_ThrowsException()
    {
        var sessionId = "test-session";
        string? message = null;

        Func<Task> act = () => _chatService.ProcessMessageAsync(sessionId, message!, CancellationToken.None);

        act.Should().ThrowAsync<ArgumentNullException>();
    }
}
