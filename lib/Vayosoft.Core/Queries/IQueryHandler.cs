using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Core.Queries
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
           where TQuery : IQuery<TResponse>
    { }

    public interface IStreamQueryHandler<in TQuery, out TResponse> : IStreamRequestHandler<TQuery, TResponse>
        where TQuery : IStreamQuery<TResponse>
    { }

    public static class QueryHandlerConfiguration
    {
        public static IServiceCollection AddQueryHandler<T, TResult, TQueryHandler>(
            this IServiceCollection services,
            Func<IServiceProvider, TQueryHandler> configure = null
        ) where TQueryHandler : class, IQueryHandler<T, TResult> where T : IQuery<TResult>
        {
            if (configure == null)
            {
                services.AddTransient<TQueryHandler, TQueryHandler>();
                services.AddTransient<IRequestHandler<T, TResult>, TQueryHandler>();
            }
            else
            {
                services.AddTransient<TQueryHandler, TQueryHandler>(configure);
                services.AddTransient<IRequestHandler<T, TResult>, TQueryHandler>(configure);
            }

            return services;
        }

        public static IServiceCollection AddStreamQueryHandler<T, TResult, TQueryHandler>(
            this IServiceCollection services,
            Func<IServiceProvider, TQueryHandler> configure = null
        ) where TQueryHandler : class, IStreamQueryHandler<T, TResult> where T : IStreamQuery<TResult>
        {
            if (configure == null)
            {
                services.AddTransient<TQueryHandler, TQueryHandler>();
                services.AddTransient<IStreamRequestHandler<T, TResult>, TQueryHandler>();
            }
            else
            {
                services.AddTransient<TQueryHandler, TQueryHandler>(configure);
                services.AddTransient<IStreamRequestHandler<T, TResult>, TQueryHandler>(configure);
            }

            return services;
        }
    }
}
