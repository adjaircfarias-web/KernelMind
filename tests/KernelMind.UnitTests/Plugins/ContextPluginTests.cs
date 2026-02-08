using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ContextPlugin = KernelMind.Core.Plugins.ContextPlugin;

namespace KernelMind.UnitTests.Plugins;

public class ContextPluginTests
{
    private readonly Mock<ILogger<ContextPlugin>> _loggerMock;
    private readonly ContextPlugin _contextPlugin;

    public ContextPluginTests()
    {
        _loggerMock = new Mock<ILogger<ContextPlugin>>();
        _contextPlugin = new ContextPlugin(_loggerMock.Object);
    }

    [Fact]
    public void SetContext_WithValidData_ReturnsConfirmation()
    {
        var result = _contextPlugin.SetContext("session123", "customer_name", "John Doe");

        result.Should().Contain("armazenada");
    }

    [Fact]
    public void GetContext_WithValidKey_ReturnsValue()
    {
        _contextPlugin.SetContext("session123", "customer_name", "John Doe");

        var result = _contextPlugin.GetContext("session123", "customer_name");

        result.Should().Contain("John Doe");
    }

    [Fact]
    public void GetContext_WithInvalidKey_ReturnsNotFound()
    {
        var result = _contextPlugin.GetContext("session123", "invalid_key");

        result.Should().Contain("Nenhuma informação encontrada");
    }

    [Fact]
    public void ClearContext_WithValidSession_ReturnsClearedMessage()
    {
        _contextPlugin.SetContext("session123", "key", "value");

        var result = _contextPlugin.ClearContext("session123");

        result.Should().Contain("limpo");
    }

    [Fact]
    public void GetConversationSummary_WithData_ReturnsSummary()
    {
        _contextPlugin.SetContext("session123", "customer_name", "John Doe");

        var result = _contextPlugin.GetConversationSummary("session123");

        result.Should().Contain("Resumo da Conversa");
        result.Should().Contain("John Doe");
    }

    [Fact]
    public void MultipleSessions_AreIsolated()
    {
        _contextPlugin.SetContext("session1", "key", "value1");
        _contextPlugin.SetContext("session2", "key", "value2");

        var result1 = _contextPlugin.GetContext("session1", "key");
        var result2 = _contextPlugin.GetContext("session2", "key");

        result1.Should().Contain("value1");
        result2.Should().Contain("value2");
    }
}
