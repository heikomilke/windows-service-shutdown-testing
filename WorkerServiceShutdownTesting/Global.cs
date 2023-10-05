public class Global
{

    public static string ServiceName => "XXX Shutdown Service Test";
    /// <summary>
    /// set by installer after it has been created
    /// </summary>
    public static string DataFolder { get; set; }

    public static string DatabasePath { get; set; }
    
    public static string LogTableName { get; set; }
}