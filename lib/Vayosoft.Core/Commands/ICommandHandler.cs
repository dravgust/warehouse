using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Core.Commands
{
    public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest>
        where TRequest : ICommand
    { }

    public interface ICommandHandler<in TRequest, TOutput>: IRequestHandler<TRequest, TOutput>
        where TRequest : ICommand<TOutput>
    { }

    public static class CommandHandlerConfiguration
    {
        public static IServiceCollection AddCommandHandler<T, TCommandHandler>(
            this IServiceCollection services,
            Func<IServiceProvider, TCommandHandler> configure = null
        ) where TCommandHandler : class, ICommandHandler<T> where T : ICommand
        {

            if (configure == null)
            {
                services.AddTransient<TCommandHandler, TCommandHandler>();
                services.AddTransient<IRequestHandler<T, Unit>, TCommandHandler>();
            }
            else
            {
                services.AddTransient<TCommandHandler, TCommandHandler>(configure);
                services.AddTransient<IRequestHandler<T, Unit>, TCommandHandler>(configure);
            }

            return services;
        }

        public static IServiceCollection AddCommandHandler<T, TOutput, TCommandHandler>(
            this IServiceCollection services,
            Func<IServiceProvider, TCommandHandler> configure = null
        ) where TCommandHandler : class, ICommandHandler<T, TOutput> where T : ICommand<TOutput>
        {

            if (configure == null)
            {
                services.AddTransient<TCommandHandler, TCommandHandler>();
                services.AddTransient<IRequestHandler<T, TOutput>, TCommandHandler>();
            }
            else
            {
                services.AddTransient<TCommandHandler, TCommandHandler>(configure);
                services.AddTransient<IRequestHandler<T, TOutput>, TCommandHandler>(configure);
            }

            return services;
        }
    }
}
