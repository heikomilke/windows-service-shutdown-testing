using WorkerServiceShutdownTesting;

IHost host = Host.CreateDefaultBuilder(args)
    // The method UseWindowsService will take care of all the configuration for you within Windows.
    .UseWindowsService(configure: configure =>
        configure.ServiceName = "XXX Shutdown Service Test"
    )
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .Build();

host.Run();