using Microsoft.Data.Sqlite;

namespace WorkerServiceShutdownTesting.Data;

public static class DatabaseHelper
{
    public static bool TableExists(SqliteConnection connection, string tableName)
    {
        string query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";
        using SqliteCommand command = new SqliteCommand(query, connection);
        object? result = command.ExecuteScalar();
        return result != null && result.ToString() == tableName;
    }

    public static void CreateLogTable(SqliteConnection connection, string logTableName)
    {
        var query = $@"CREATE TABLE IF NOT EXISTS ""{logTableName}"" (
        ""ID"" INTEGER PRIMARY KEY AUTOINCREMENT,
        ""DateCreate"" DATETIME DEFAULT CURRENT_TIMESTAMP,
        ""Message"" TEXT
            );";
        
        using SqliteCommand command = new(query, connection);
        command.ExecuteNonQuery();
    }
    
}