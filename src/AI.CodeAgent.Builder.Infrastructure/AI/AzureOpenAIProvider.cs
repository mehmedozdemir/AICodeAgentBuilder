using AI.CodeAgent.Builder.Application.Common.Interfaces.AI;
using AI.CodeAgent.Builder.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.CodeAgent.Builder.Infrastructure.AI;

/// <summary>
/// Azure OpenAI provider implementation using Azure-specific endpoints.
/// </summary>
public sealed class AzureOpenAIProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly AIProviderSettings _settings;

    public string ProviderName => "Azure OpenAI";
    public string ModelName { get; }

    public AzureOpenAIProvider(HttpClient httpClient, IOptions<AIProviderSettings> settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

        _httpClient.BaseAddress = new Uri(_settings.AzureOpenAI.Endpoint);
        _httpClient.DefaultRequestHeaders.Add("api-key", _settings.AzureOpenAI.ApiKey);

        ModelName = _settings.AzureOpenAI.DeploymentName;
    }

    public async Task<AIPromptResponse> SendPromptAsync(AIPromptRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var messages = new List<AzureOpenAIMessage>();

            if (!string.IsNullOrWhiteSpace(request.SystemMessage))
            {
                messages.Add(new AzureOpenAIMessage { Role = "system", Content = request.SystemMessage });
            }

            messages.Add(new AzureOpenAIMessage { Role = "user", Content = request.Prompt });

            var requestBody = new AzureOpenAIRequest
            {
                Messages = messages,
                Temperature = request.Temperature ?? _settings.DefaultTemperature,
                MaxTokens = request.MaxTokens ?? _settings.DefaultMaxTokens
            };

            var apiVersion = _settings.AzureOpenAI.ApiVersion ?? "2024-02-15-preview";
            var url = $"/openai/deployments/{_settings.AzureOpenAI.DeploymentName}/chat/completions?api-version={apiVersion}";

            var response = await _httpClient.PostAsJsonAsync(
                url,
                requestBody,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AzureOpenAIResponse>(cancellationToken);

            if (result == null || result.Choices == null || result.Choices.Count == 0)
            {
                return AIPromptResponse.Failure("No response from Azure OpenAI", stopwatch.Elapsed);
            }

            var content = result.Choices[0].Message?.Content ?? string.Empty;

            return AIPromptResponse.Success(
                content,
                result.Usage?.PromptTokens,
                result.Usage?.CompletionTokens,
                result.Usage?.TotalTokens,
                stopwatch.Elapsed,
                _settings.AzureOpenAI.DeploymentName);
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

    // Internal DTOs for Azure OpenAI API
    private sealed class AzureOpenAIRequest
    {
        [JsonPropertyName("messages")]
        public List<AzureOpenAIMessage> Messages { get; set; } = new();

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }
    }

    private sealed class AzureOpenAIMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private sealed class AzureOpenAIResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public List<AzureOpenAIChoice>? Choices { get; set; }

        [JsonPropertyName("usage")]
        public AzureOpenAIUsage? Usage { get; set; }
    }

    private sealed class AzureOpenAIChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public AzureOpenAIMessage? Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    private sealed class AzureOpenAIUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
