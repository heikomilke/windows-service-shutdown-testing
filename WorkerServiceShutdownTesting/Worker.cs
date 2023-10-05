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

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        
        await _writer.LogMessageAsync("I came through stop");
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _writer.ConnectAsync(stoppingToken);
        await _writer.LogMessageAsync("Connection made with new service");
        long seq = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            await _writer.LogMessageAsync($"Again {seq++}");
            _logger.LogInformation("Worker running at: {0}, btw my data folder is : {1}", DateTimeOffset.Now, Environment.GetEnvironmentVariable("ProgramData"));
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception delayEx)
            {
                await _writer.LogMessageAsync("Caught delay ex: "+ delayEx);
            }
        }

        await _writer.LogMessageAsync($"Exiting loop. Token is: {stoppingToken}");
    }
}