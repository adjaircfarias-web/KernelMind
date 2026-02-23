namespace KernelMind.Core.Configuration;

public class OllamaOptions
{
    public const string Ollama = "Ollama";

    public string Host { get; set; } = "http://localhost:11434";
    public string ChatModel { get; set; } = "llama3.1:70b";
    public string EmbeddingModel { get; set; } = "nomic-embed-text";
}
