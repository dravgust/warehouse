
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Serilog;
using Warehouse.API;
using Warehouse.API.Resources;
using Warehouse.API.Services.Authorization;
using Warehouse.API.Services.ExceptionHandling;
using Warehouse.API.Services.Monitoring;
using Warehouse.API.Services.Swagger;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.AddServerHeader = false;
    });

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

    var configuration = builder.Configuration;

    builder.Services
        .AddApplication(configuration) //builder.Environment.IsDevelopment()
        .AddIdentityService(configuration)
        .AddLocalizationService();

    builder.Services
        .AddHealthChecks()
        .AddRedis(configuration["ConnectionStrings:RedisConnectionString"], tags: new[] { "infrastructure" })
        .AddMySql(configuration["ConnectionStrings:DefaultConnection"], tags: new[] { "infrastructure" })
        .AddMongoDb(configuration["MongoDbContext:ConnectionString"], tags: new[] { "infrastructure" });
    //services.AddAppMetricsCollectors();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "AllowCors",
            policy =>
            {
                policy
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader().AllowAnyMethod()
                    .AllowCredentials();
            });
    });

    // Add services to the container.
    builder.Services.AddControllersWithViews(options =>
    { /*options.Filters.Add(new AuthorizeAttribute());*/ })
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
    options => { options.ResourcesPath = "Resources"; })
    .AddDataAnnotationsLocalization(resOptions =>
    {
        resOptions.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResources));
    });

    builder.Services.AddApiVersioningService();
    builder.Services.AddSwaggerService();

    //builder.Services.AddMemoryCache();
    //builder.Services.AddDistributedMemoryCache();
    //builder.Services.Configure<RedisCacheOptions>(options =>
    //{
    //    options.Configuration = configuration.GetConnectionString("RedisConnectionString");
    //    options.InstanceName = "SessionInstance";
    //});
    //builder.Services.AddSingleton<IDistributedCache, RedisCache>();
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = configuration.GetConnectionString("RedisConnectionString");
        options.InstanceName = "SessionInstance";
    });
    builder.Services.AddSession(options =>
    {
        options.Cookie.Name = ".session";
        options.IdleTimeout = TimeSpan.FromSeconds(3600); //Default is 20 minutes.
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = false;
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

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseStaticFiles();

    // Write streamlined request completion events, instead of the more verbose ones from the framework.
    // To use the default framework request logging instead, remove this line and set the "Microsoft"
    // level in appsettings.json to "Information".
    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseCors("AllowCors");

    app.UseAuthorization();
    app.UseSession();

    // app.UseResponseCaching();
    app.UseMiddleware<JwtMiddleware>();

    var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
    app.UseRequestLocalization(locOptions!.Value);

    app.MapHealthChecks("/health", new HealthCheckOptions()
    {
        AllowCachingResponses = false,
        ResponseWriter = HealthCheckResponse.Write
    });
    app.MapHealthChecks("/live", new HealthCheckOptions()
    {
        Predicate = (check) => check.Tags.Contains("infrastructure"),
        ResponseWriter = HealthCheckResponse.WriteRaw
    });

    //app.MapControllerRoute(
    //    name: "default",
    //    pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapControllerRoute(
        name: "api",
        pattern: "/api/{controller=Home}/{id?}");

    app.MapFallbackToFile("/index.html");

    app.UseSwaggerService();

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