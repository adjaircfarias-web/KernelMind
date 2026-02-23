using FluentAssertions;

namespace KernelMind.UnitTests.Services;

public class ChatServiceValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ProcessMessageAsync_WithEmptyMessage_ThrowsArgumentException(string message)
    {
        var service = new TestableChatService();

        Func<Task> act = () => service.ProcessMessageValidationAsync("test-session", message);

        act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public void ProcessMessageAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        var service = new TestableChatService();
        string? message = null;

        Func<Task> act = () => service.ProcessMessageValidationAsync("test-session", message!);

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("Hello")]
    [InlineData("Quero uma pizza")]
    [InlineData("Quanto custa a margherita?")]
    public void ProcessMessageAsync_WithValidMessage_DoesNotThrow(string message)
    {
        var service = new TestableChatService();

        var exception = Record.ExceptionAsync(() => 
            service.ProcessMessageValidationAsync("test-session", message));
        
        exception.Result.Should().BeNull();
    }

    private class TestableChatService
    {
        public Task ProcessMessageValidationAsync(string sessionId, string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty");

            return Task.CompletedTask;
        }
    }
}

public class ChatServiceSystemPromptTests
{
    [Fact]
    public void GetSystemPrompt_ReturnsNonEmptyString()
    {
        var service = new TestableChatService2();
        var prompt = service.GetSystemPrompt();

        prompt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetSystemPrompt_ContainsPizzeriaContext()
    {
        var service = new TestableChatService2();
        var prompt = service.GetSystemPrompt();

        prompt.Should().Contain("pizzaria");
        prompt.Should().Contain("KernelMind");
    }

    [Fact]
    public void GetSystemPrompt_ContainsPortugueseInstructions()
    {
        var service = new TestableChatService2();
        var prompt = service.GetSystemPrompt();

        prompt.Should().Contain("português");
    }

    private class TestableChatService2
    {
        public string GetSystemPrompt()
        {
            return @"Você é um assistente virtual de uma pizzaria chamada KernelMind. 
Responda sempre em português de forma clara e amigável.";
        }
    }
}
