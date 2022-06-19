using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vayosoft.Core.SharedKernel.Commands;
using Vayosoft.Core.SharedKernel.Commands.External;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Core.SharedKernel.Queries;

namespace Vayosoft.Core
{
    public static class Config
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddMediatR()
                .AddScoped<ICommandBus, CommandBus>()
                .AddScoped<IQueryBus, QueryBus>();

            services.TryAddScoped<IEventBus, EventBus>();
            services.TryAddScoped<IExternalEventProducer, NullExternalEventProducer>();
            services.TryAddScoped<IExternalCommandBus, ExternalCommandBus>();

            return services;
        }

        private static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            return services.AddScoped<IMediator, Mediator>()
                .AddTransient<ServiceFactory>(sp => sp.GetRequiredService!);
        }
    }
}
