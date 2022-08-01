using System.Diagnostics;
using Serilog;
using Warehouse.Host;

//https://code-maze.com/aspnetcore-running-applications-as-windows-service/
//https://csharp.christiannagel.com/2022/03/22/windowsservice-2/
//sc create "WarehouseHostService" binPath="C:\Vayosoft\host\Warehouse.Host.exe"

Log.Logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .CreateBootstrapLogger();

try
{
    IHost host = Host.CreateDefaultBuilder(args)
        .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
#if DEBUG
                // Used to filter out potentially bad data due debugging.
                // Very useful when doing Seq dashboards and want to remove logs under debugging session.
                .Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached)
#endif
        )
        .UseWindowsService(options => { options.ServiceName = "WarehouseHostService"; })
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            services.AddApplication(configuration);

        }).Build();

    await host.RunAsync();
}
catch (Exception e)
{
    Log.Fatal(e, "An unhandled exception occurred during bootstrapping");
    throw;
}
finally
{
    Log.CloseAndFlush();
}