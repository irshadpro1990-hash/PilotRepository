using System.Data.SqlClient;
using TransactionMonitor.Models;
using TransactionMonitor.Services;

namespace TransactionMonitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly TeamsNotifier _teamsNotifier;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, TeamsNotifier teamsNotifier)
    {
        _logger = logger;
        _configuration = configuration;
        _teamsNotifier = teamsNotifier;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                var stuckTransactions = await GetStuckTransactionsAsync();
                if (stuckTransactions.Count > 0)
                {
                    var message = $"Found {stuckTransactions.Count} stuck transactions. Transaction IDs: {string.Join(", ", stuckTransactions.Select(t => t.TransactionId))}";
                    await _teamsNotifier.SendNotificationAsync(message);
                }
                else
                {
                    _logger.LogInformation("No stuck transactions found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking for stuck transactions.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task<List<Transaction>> GetStuckTransactionsAsync()
    {
        var transactions = new List<Transaction>();
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(
                "SELECT TransactionId, Status, LastModifiedDate FROM transactionbase WHERE Status IN ('Processing', 'Pending') AND LastModifiedDate < DATEADD(minute, -5, GETDATE())",
                connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    transactions.Add(new Transaction
                    {
                        TransactionId = reader.GetInt32(0),
                        Status = reader.GetString(1),
                        LastModifiedDate = reader.GetDateTime(2)
                    });
                }
            }
        }

        return transactions;
    }
}
