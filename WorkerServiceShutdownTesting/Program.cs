using WorkerServiceShutdownTesting;
using WorkerServiceShutdownTesting.Data;

IHost host = Host.CreateDefaultBuilder(args)
    // The method UseWindowsService will take care of all the configuration for you within Windows.
    .UseWindowsService(configure: configure =>
        configure.ServiceName = Global.ServiceName
    )
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<Worker>()
            .AddSingleton<DatabaseWriter>()
            .AddTransient<Installer>();
    })
    .Build();

host.Run();