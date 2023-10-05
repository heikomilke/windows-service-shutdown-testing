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
        await _writer.LogMessageAsync("I came through stop. Reason: ");
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
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception delayEx)
            {
                await _writer.LogMessageAsync("Caught delay ex: " + delayEx);
            }
        }


        await _writer.LogMessageAsync($"Exiting loop. Token is: {stoppingToken}");
        await LogProcesses();

        // now comes the evil part ... we try to stay alive for a few seconds and keep logging processes
        try
        {
            for (int i = 0; i < 10; i++)
            {
                // we intentionally avoid use of cancel token here .. we aim to stay alive through this
                // ReSharper disable once MethodSupportsCancellation
                try
                {
                    await Task.Delay(500);
                }
                catch
                {
                    await _writer.LogMessageAsync("Failed to delay :(");
                    // ignore
                }

                await _writer.LogMessageAsync($"Stretch {i}");
                await LogProcesses();
            }
        }
        catch (Exception evilEx)
        {
            await _writer.LogMessageAsync($"Caught evilEx {evilEx}");
        }
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

                errors.Add($"Failed to log a process. Name: {nameornothing}, Ex: {ex}");
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