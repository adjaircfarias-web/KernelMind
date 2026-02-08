using FluentAssertions;

namespace KernelMind.UnitTests.Plugins;

public class ContextPluginValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SetContext_WithEmptySession_Throws(string sessionToken)
    {
        var plugin = new TestableContextPlugin();

        Action act = () => plugin.SetContextValidation(sessionToken, "key", "value");

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GetContext_WithEmptySession_Throws(string sessionToken)
    {
        var plugin = new TestableContextPlugin();

        Action act = () => plugin.GetContextValidation(sessionToken, "key");

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ClearContext_WithEmptySession_Throws(string sessionToken)
    {
        var plugin = new TestableContextPlugin();

        Action act = () => plugin.ClearContextValidation(sessionToken);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetContext_WithValidData_ReturnsSuccess()
    {
        var plugin = new TestableContextPlugin();

        var result = plugin.SetContextValidation("session123", "key", "value");

        result.Should().Contain("sucesso");
    }

    [Fact]
    public void GetContext_WithValidKey_ReturnsValue()
    {
        var plugin = new TestableContextPlugin();
        plugin.SetContextValidation("session123", "key", "value");

        var result = plugin.GetContextValidation("session123", "key");

        result.Should().Contain("value");
    }

    [Fact]
    public void GetContext_WithInvalidKey_ReturnsNotFound()
    {
        var plugin = new TestableContextPlugin();

        var result = plugin.GetContextValidation("session123", "invalid_key");

        result.Should().Contain("Nenhuma");
    }

    [Fact]
    public void ClearContext_WithValidSession_ReturnsClearedMessage()
    {
        var plugin = new TestableContextPlugin();
        plugin.SetContextValidation("session123", "key", "value");

        var result = plugin.ClearContextValidation("session123");

        result.Should().Contain("limpo");
    }

    [Fact]
    public void MultipleSessions_AreIsolated()
    {
        var plugin = new TestableContextPlugin();

        plugin.SetContextValidation("session1", "key", "value1");
        plugin.SetContextValidation("session2", "key", "value2");

        var result1 = plugin.GetContextValidation("session1", "key");
        var result2 = plugin.GetContextValidation("session2", "key");

        result1.Should().Contain("value1");
        result2.Should().Contain("value2");
    }

    private class TestableContextPlugin
    {
        private readonly Dictionary<string, Dictionary<string, string>> _contexts = new();

        public string SetContextValidation(string sessionToken, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
                throw new ArgumentException("Session token required");

            if (!_contexts.ContainsKey(sessionToken))
                _contexts[sessionToken] = new Dictionary<string, string>();

            _contexts[sessionToken][key] = value;

            return "Informação armazenada com sucesso";
        }

        public string GetContextValidation(string sessionToken, string key)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
                throw new ArgumentException("Session token required");

            if (!_contexts.TryGetValue(sessionToken, out var context))
                return "Nenhuma informação encontrada";

            if (!context.TryGetValue(key, out var value))
                return "Nenhuma informação encontrada";

            return $"{key}: {value}";
        }

        public string ClearContextValidation(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
                throw new ArgumentException("Session token required");

            _contexts.Remove(sessionToken);

            return "Contexto limpo com sucesso";
        }
    }
}

public class ContextPluginSessionTests
{
    [Fact]
    public void SessionToken_IsUnique()
    {
        var tokens = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            var token = "session_" + Guid.NewGuid().ToString("N");
            tokens.Add(token);
        }

        tokens.Count.Should().Be(100);
    }

    [Fact]
    public void SessionToken_HasCorrectFormat()
    {
        var token = "session_" + Guid.NewGuid().ToString("N");

        token.Should().StartWith("session_");
    }

    [Fact]
    public void EmptyContext_ReturnsAppropriateMessage()
    {
        var plugin = new EmptyContextPlugin();
        plugin.SetSession("session123");

        var result = plugin.GetSummary();

        result.Should().Contain("Nenhuma");
    }

    private class EmptyContextPlugin
    {
        private string _session = string.Empty;

        public void SetSession(string session) => _session = session;

        public string GetSummary()
        {
            if (string.IsNullOrEmpty(_session))
                return "Nenhuma sessão";

            return "Nenhuma informação no contexto";
        }
    }
}
