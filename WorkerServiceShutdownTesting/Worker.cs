namespace WorkerServiceShutdownTesting;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, Installer installer)
    {
        _logger = logger;
        installer.EnsureInstalled();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return;
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {0}, btw my data folder is : {1}", DateTimeOffset.Now, Environment.GetEnvironmentVariable("ProgramData"));
            await Task.Delay(1000, stoppingToken);
        }
    }
}