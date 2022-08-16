namespace Warehouse.API.Services.Swagger
{
    public static class Configuration
    {
        public static IServiceCollection AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerGenOptions>();

            return services;
        }

        public static IApplicationBuilder UseSwaggerService(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();
            app.UseSwagger(c => c.RouteTemplate = "/swagger/api/{documentName}/swagger.json");

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/api/v1/swagger.json", "IPS Dashboard V1");
                c.InjectStylesheet("/css/swagger.css");
                c.RoutePrefix = "api";
            });

            return app;
        }
    }
}
