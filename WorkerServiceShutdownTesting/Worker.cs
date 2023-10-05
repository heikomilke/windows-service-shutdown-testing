using WorkerServiceShutdownTesting.Data;

namespace WorkerServiceShutdownTesting;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DatabaseWriter _writer;

    public Worker(ILogger<Worker> logger, Installer installer, DatabaseWriter writer)
    {
        _logger = logger;
        _writer = writer;
        installer.EnsureInstalled();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _writer.ConnectAsync(stoppingToken);
        await _writer.LogMessageAsync("Connection made");
        long seq = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            await _writer.LogMessageAsync("Again " + seq++);
            _logger.LogInformation("Worker running at: {0}, btw my data folder is : {1}", DateTimeOffset.Now, Environment.GetEnvironmentVariable("ProgramData"));
            await Task.Delay(1000, stoppingToken);
        }
    }
}