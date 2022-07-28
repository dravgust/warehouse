using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Core.Queries
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
           where TQuery : IQuery<TResponse>
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
    }
}
