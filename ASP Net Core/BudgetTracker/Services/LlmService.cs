using System.Text;
using System.Text.Json;
using BudgetTracker.Models.DTOs.Llm;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services
{
    /// <summary>
    /// Service for communicating with the Python LLM microservice
    /// </summary>
    public class LlmService : ILlmService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LlmService> _logger;
        private readonly string _baseUrl;

        public LlmService(HttpClient httpClient, IConfiguration configuration, ILogger<LlmService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = configuration["LlmService:BaseUrl"] ?? "http://localhost:8000";
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(120); // LLM can take time
        }

        public async Task<SummaryResponseDto?> GetExpenseSummaryAsync(SummaryRequestDto request)
        {
            try
            {
                _logger.LogInformation("Requesting expense summary from LLM service with {Count} expenses", request.Expenses.Count);
                
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/summarize", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<SummaryResponseDto>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    _logger.LogInformation("Successfully received summary from LLM service");
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("LLM service returned error {StatusCode}: {Error}", response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to LLM service at {BaseUrl}", _baseUrl);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request to LLM service timed out");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling LLM service");
                return null;
            }
        }

        public async Task<AdviceResponseDto?> GetBudgetingAdviceAsync(AdviceRequestDto request)
        {
            try
            {
                _logger.LogInformation("Requesting budgeting advice from LLM service: {Question}", request.Question);
                
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/advice", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AdviceResponseDto>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    _logger.LogInformation("Successfully received advice from LLM service");
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("LLM service returned error {StatusCode}: {Error}", response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to LLM service at {BaseUrl}", _baseUrl);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request to LLM service timed out");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling LLM service");
                return null;
            }
        }

        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}

