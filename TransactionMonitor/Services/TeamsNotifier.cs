using System.Text;
using System.Text.Json;

namespace TransactionMonitor.Services;

public class TeamsNotifier
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TeamsNotifier> _logger;

    public TeamsNotifier(HttpClient httpClient, IConfiguration configuration, ILogger<TeamsNotifier> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendNotificationAsync(string message)
    {
        var webhookUrl = _configuration["TeamsWebhookUrl"];
        if (string.IsNullOrEmpty(webhookUrl))
        {
            _logger.LogError("Teams webhook URL is not configured.");
            return;
        }

        var payload = new { text = message };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(webhookUrl, content);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Successfully sent notification to Teams.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to Teams.");
        }
    }
}
