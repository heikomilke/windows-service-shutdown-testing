using System.Diagnostics;
using System.Text;
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
        
        await _writer.LogMessageAsync("I came through stop. Reason: " );
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _writer.ConnectAsync(stoppingToken);
        await _writer.LogMessageAsync("Connection made with new service");
        await LogProcesses();

        long seq = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            //await _writer.LogMessageAsync($"Again {seq++}");
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
        await LogProcesses();
    }

    private async Task LogProcesses()
    {

        StringBuilder sb = new();
        List<string> errors = new();
        sb.AppendFormat("Processes:");
        Process[] processes = Process.GetProcesses();
        foreach (Process process in processes)
        {
            try
            {
                
                sb.AppendFormat("{0,-30} {1,-10} {2}", process.ProcessName, process.Id, GetProcessPath(process));
            }
            catch (Exception ex)
            {
                string nameornothing = "n/a";
                try
                {
                    nameornothing = process.ProcessName;
                }
                catch
                {
                    //ignored
                }
                errors.Add($"Failed to log a process. Name: {nameornothing}, Ex: {ex}" );
            }

            sb.AppendLine();
        }

        if (errors.Any())
            await _writer.LogMessageAsync("Got errors: " + string.Join("\r\n", errors));
        await _writer.LogMessageAsync(sb.ToString());

    }
    
    // security might prevent us from getting path so lets try catch it here
    static string GetProcessPath(Process process)
    {
        try
        {
            return process.MainModule.FileName;
        }
        catch (Exception)
        {
            return "(hidden)";
        }
    }
}