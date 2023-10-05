using Microsoft.Data.Sqlite;

namespace WorkerServiceShutdownTesting.Data;

public class DatabaseWriter : IDisposable
{
    private readonly ILogger<DatabaseWriter> _logger;
    private SqliteConnection _connection;
    private readonly string _connectionString;

    public DatabaseWriter(ILogger<DatabaseWriter> logger)
    {
        _logger = logger;

        _connection = new SqliteConnection();
    }

    public async Task ConnectAsync(CancellationToken stoppingToken)
    {
        _connection.ConnectionString = $"Data Source={Global.DatabasePath}";

        await _connection.OpenAsync(stoppingToken);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public async Task LogMessageAsync(string message)
    {
        var query = $@"INSERT INTO ""{Global.LogTableName}"" (""Message"") VALUES (@Message)";
        var cmd = _connection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@Message", message);
        await cmd.ExecuteNonQueryAsync();
    }
}