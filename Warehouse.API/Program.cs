
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Vayosoft.Caching;
using Vayosoft.Data.Redis;
using Warehouse.API.Resources;
using Warehouse.API.Services.ExceptionHandling;
using Warehouse.API.Services.Localization;
using Warehouse.API.Services.Security;
using Warehouse.API.TagHelpers;
using Warehouse.API.UseCases.Resources;
using Warehouse.Core;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Validation;
using Warehouse.Core.UseCases.Administration.Models;

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

    builder.Services
        //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SetProduct.CertificateRequestValidator>())
        .AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Warehouse.Core.Services.Validation.Config)), ServiceLifetime.Transient)
        .AddValidation();
    builder.Services.AddUnhandledException();

    builder.Services.AddWarehouseDependencies(configuration);
    builder.Services.AddSingleton<IRequestHandler<GetResources, IEnumerable<ResourceGroup>>, GetResources.ResourcesQueryHandler>();

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

    //builder.Services.AddAuthorization(options =>
    //{
    //    options.AddPolicy("Over18",
    //        policy => policy.Requirements.Add(new Over18Requirement()));
    //});

    // Add services to the container.
    builder.Services.AddControllersWithViews(options =>
    {
        //options.Filters.Add(new AuthorizeAttribute());
    })
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
    options =>
    {
        options.ResourcesPath = "Resources";
    })
    .AddDataAnnotationsLocalization(resOptions =>
    {
        resOptions.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResources));
    });

    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
    builder.Services.AddSingleton<SharedLocalizationService>();

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[]
        {
            new CultureInfo("en"),
            new CultureInfo("he"),
        };

        options.DefaultRequestCulture = new RequestCulture("he");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.ApplyCurrentCultureToResponseHeaders = true;
    });

    // configure strongly typed settings object
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IPasswordHasher, MD5PasswordHasher>();
    builder.Services.AddScoped<IIdentityUserStore, IdentityUserStore>();
    builder.Services.AddScoped<IIdentityUserService, IdentityUserService>();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "IPS Dashboard", Version = "v1" });
        c.AddSecurityDefinition("Bearer",
            new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,

                },
                new List<string>()
            }
        });
        //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        //c.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddRedisConnection();
    builder.Services.AddCaching(configuration);
    //builder.Services.AddRedisCache(configuration);
    //builder.Services.AddMemoryCache();
    //builder.Services.AddDistributedMemoryCache();

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

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
    app.UseRequestLocalization(locOptions!.Value);

    app.UseStaticFiles();

    // Write streamlined request completion events, instead of the more verbose ones from the framework.
    // To use the default framework request logging instead, remove this line and set the "Microsoft"
    // level in appsettings.json to "Information".
    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseCors("AllowCors");

    app.UseAuthorization();
    //app.UseSession();
    // app.UseResponseCaching();

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