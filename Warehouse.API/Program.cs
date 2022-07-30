
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Serilog;
using Warehouse.API;
using Warehouse.API.Resources;
using Warehouse.API.Services.ExceptionHandling;
using Warehouse.API.Services.Security;

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

    var configuration = builder.Configuration;

    builder.Services
        .AddApplication(configuration)
        .AddIdentityService(configuration)
        .AddLocalizationService();

    builder.Services.AddSwaggerService();
    builder.Services.AddHealthChecks();
    //services.AddAppMetricsCollectors();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "AllowCors",
            policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
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

    //builder.Services.AddMemoryCache();
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(10); //Set Session Timeout. Default is 20 minutes.
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
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
    //**********************************************
    //app.Use(async (context, next) =>
    //{
    //    context.Items.Add("message", "Hello METANIT.COM");
    //    await next.Invoke();
    //});

    //app.Run(async (context) =>
    //{
    //    if (context.Items.ContainsKey("message"))
    //        await context.Response.WriteAsync($"Message: {context.Items["message"]}");
    //    else
    //        await context.Response.WriteAsync("Random Text");
    //});
    //app.Run(async (context) =>
    //{
    //    if (context.Session.Keys.Contains("name"))
    //        await context.Response.WriteAsync($"Hello {context.Session.GetString("name")}!");
    //    else
    //    {
    //        context.Session.SetString("name", "Tom");
    //        await context.Response.WriteAsync("Hello World!");
    //    }
    //});
    //**********************************************
    // app.UseResponseCaching();
    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
    app.UseRequestLocalization(locOptions!.Value);

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

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