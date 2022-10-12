using Microsoft.AspNetCore.Mvc;
using Warehouse.API.Controllers.API;

namespace Warehouse.API.Endpoints;

public static class EndpointBaseAsync
{
    public static class WithRequest<TRequest>
    {
        public abstract class WithResult<TResponse> : ApiControllerBase
        {
            public abstract Task<TResponse> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default(CancellationToken));
        }

        public abstract class WithoutResult : ApiControllerBase
        {
            public abstract Task HandleAsync(TRequest request, CancellationToken cancellationToken = default(CancellationToken));
        }

        public abstract class WithActionResult<TResponse> : ApiControllerBase
        {
            public abstract Task<ActionResult<TResponse>> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default(CancellationToken));
        }

        public abstract class WithActionResult : ApiControllerBase
        {
            public abstract Task<ActionResult> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default(CancellationToken));
        }
    }

    public static class WithoutRequest
    {
        public abstract class WithResult<TResponse> : ApiControllerBase
        {
            public abstract Task<TResponse> HandleAsync(CancellationToken cancellationToken = default(CancellationToken));
        }

        public abstract class WithoutResult : ApiControllerBase
        {
            public abstract Task HandleAsync(CancellationToken cancellationToken = default(CancellationToken));
        }

        public abstract class WithActionResult<TResponse> : ApiControllerBase
        {
            public abstract Task<ActionResult<TResponse>> HandleAsync(
                CancellationToken cancellationToken = default(CancellationToken));
        }

        public abstract class WithActionResult : ApiControllerBase
        {
            public abstract Task<ActionResult> HandleAsync(
                CancellationToken cancellationToken = default(CancellationToken));
        }
    }
}