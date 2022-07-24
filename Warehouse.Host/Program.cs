using Microsoft.Extensions.Logging.EventLog;
using Warehouse.Host;

//https://code-maze.com/aspnetcore-running-applications-as-windows-service/
//https://csharp.christiannagel.com/2022/03/22/windowsservice-2/
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(options =>
    {
        if (OperatingSystem.IsWindows())
        {
            options.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information);
        }
    })
    .UseWindowsService(options =>
    {
        options.ServiceName = "Warehouse Host Service";
    })
    .ConfigureServices(services =>
    {
        //services.AddHostedService<Worker>();
        services.AddHostedService<HostedService>();

        if (OperatingSystem.IsWindows())
        {
            services.Configure<EventLogSettings>(config =>
            {
                //config.LogName = "Warehouse Host Service";
                //config.SourceName = "Warehouse Host";
            });
        }
    })
    .Build();

await host.RunAsync();
