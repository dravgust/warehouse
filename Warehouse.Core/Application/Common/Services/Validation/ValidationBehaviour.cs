using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Vayosoft.Utilities;

namespace Warehouse.Core.Application.Common.Services.Validation
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                string typeName = request.GetGenericTypeName();

                ValidationContext<TRequest> context = new(request);
                ValidationResult[] validationResults =
                    await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                List<ValidationFailure> failures = validationResults.SelectMany(result => result.Errors)
                    .Where(error => error != null).ToList();
                if (failures.Any())
                {
                    if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning(
                            "Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}",
                            typeName, request, failures);
                    }

                    //throw new ApplicationException(
                    //    $"Command Validation Errors for type {typeof(TRequest).Name}",
                    //    new ValidationException("Validation exception", failures));

                    throw new ValidationException("Validation exception", failures);
                }

            }
            return await next();
        }
    }
}
