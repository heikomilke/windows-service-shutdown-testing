using Microsoft.Data.Sqlite;
using WorkerServiceShutdownTesting.Data;

namespace WorkerServiceShutdownTesting;

/// <summary>
/// this class will make sure that the minimal requirements are met for us to run
/// will create folder and db the first time we run
/// </summary>
public class Installer
{
    private readonly ILogger<Installer> _logger;

    public Installer(ILogger<Installer> logger)
    {
        _logger = logger;
    }
    public void EnsureInstalled()
    {
        _logger.LogInformation("Ensuring we're installed");

        var dataFolderRoot = Environment.GetEnvironmentVariable("ProgramData");
        if (dataFolderRoot == null)
            throw new Exception("Unable to install. Missing ProgramData");
        Global.DataFolder = Path.Combine(dataFolderRoot, Global.ServiceName);
        Directory.CreateDirectory(Global.DataFolder);

        Global.DatabasePath = Path.Combine(Global.DataFolder, "db.sdf");


        var connectionString = $"Data Source={Global.DatabasePath}";
        using var connection = new SqliteConnection(connectionString);
        _logger.LogInformation($"Connecting to db: {connectionString}" );

        connection.Open();

        var tableExists = DatabaseHelper.TableExists(connection, Global.LogTableName);
        _logger.LogInformation("Table exists: "+ tableExists);

        if (!tableExists)
        {
            DatabaseHelper.CreateLogTable(connection, Global.LogTableName);
        }


    }
}