
using System.Diagnostics;
using IpsWeb;
using Microsoft.OpenApi.Models;
using Serilog;
using Vayosoft.Core;
using Vayosoft.WebAPI.Middlewares.ExceptionHandling;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .Enrich.WithCorrelationId()
#if DEBUG
        // Used to filter out potentially bad data due debugging.
        // Very useful when doing Seq dashboards and want to remove logs under debugging session.
        .Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached)
#endif
        );

    builder.Services.AddHealthChecks();
    //services.AddAppMetricsCollectors();

    var configuration = builder.Configuration;

    builder.Services.AddCoreServices();
    builder.Services.AddDependencies(configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "AllowCors",
            policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "IPS Dashboard", Version = "v1" });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
    app.UseStaticFiles();

    // Write streamlined request completion events, instead of the more verbose ones from the framework.
    // To use the default framework request logging instead, remove this line and set the "Microsoft"
    // level in appsettings.json to "Information".
    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseCors("AllowCors");

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("swagger/v1/swagger.json", "IPS Dashboard V1");
        c.RoutePrefix = string.Empty;
    });

    app.Run();
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