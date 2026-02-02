using AI.CodeAgent.Builder.Application.Common.Interfaces.AI;
using AI.CodeAgent.Builder.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.CodeAgent.Builder.Infrastructure.AI;

/// <summary>
/// OpenAI provider implementation using direct HTTP API calls.
/// </summary>
public sealed class OpenAIProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly AIProviderSettings _settings;
    private const string DefaultBaseUrl = "https://api.openai.com/v1";

    public string ProviderName => "OpenAI";
    public string ModelName { get; }

    public OpenAIProvider(HttpClient httpClient, IOptions<AIProviderSettings> settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

        var baseUrl = _settings.OpenAI.BaseUrl ?? DefaultBaseUrl;
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.OpenAI.ApiKey}");

        if (!string.IsNullOrWhiteSpace(_settings.OpenAI.OrganizationId))
        {
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", _settings.OpenAI.OrganizationId);
        }

        ModelName = _settings.DefaultModel;
    }

    public async Task<AIPromptResponse> SendPromptAsync(AIPromptRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var messages = new List<OpenAIMessage>();

            if (!string.IsNullOrWhiteSpace(request.SystemMessage))
            {
                messages.Add(new OpenAIMessage { Role = "system", Content = request.SystemMessage });
            }

            messages.Add(new OpenAIMessage { Role = "user", Content = request.Prompt });

            var requestBody = new OpenAIRequest
            {
                Model = ModelName,
                Messages = messages,
                Temperature = request.Temperature ?? _settings.DefaultTemperature,
                MaxTokens = request.MaxTokens ?? _settings.DefaultMaxTokens
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/chat/completions",
                requestBody,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>(cancellationToken);

            if (result == null || result.Choices == null || result.Choices.Count == 0)
            {
                return AIPromptResponse.Failure("No response from OpenAI", stopwatch.Elapsed);
            }

            var content = result.Choices[0].Message?.Content ?? string.Empty;

            return AIPromptResponse.Success(
                content,
                result.Usage?.PromptTokens,
                result.Usage?.CompletionTokens,
                result.Usage?.TotalTokens,
                stopwatch.Elapsed,
                result.Model);
        }
        catch (Exception ex)
        {
            return AIPromptResponse.Failure(ex.Message, stopwatch.Elapsed);
        }
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var testRequest = AIPromptRequest.Create("Test", "ConnectionTest");
            var response = await SendPromptAsync(testRequest, cancellationToken);
            return response.IsSuccess;
        }
        catch
        {
            return false;
        }
    }

    // Internal DTOs for OpenAI API
    private sealed class OpenAIRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<OpenAIMessage> Messages { get; set; } = new();

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }
    }

    private sealed class OpenAIMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private sealed class OpenAIResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public List<OpenAIChoice>? Choices { get; set; }

        [JsonPropertyName("usage")]
        public OpenAIUsage? Usage { get; set; }
    }

    private sealed class OpenAIChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public OpenAIMessage? Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    private sealed class OpenAIUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
