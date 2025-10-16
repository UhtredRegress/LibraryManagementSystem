using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookService.Application;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            _logger.LogInformation("Started validation for the following requests {Request}", request.GetType().Name);   
            var context = new ValidationContext<TRequest>(request);
            var validationResult = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
            var failures = validationResult.Where(f => !f.IsValid).SelectMany(f => f.Errors).ToList();

            if (failures.Count != 0)
            {
                _logger.LogError($"Validation failed for the following requests: {string.Join(", ", failures)}");
                throw new ValidationException(failures);
            }
        }
        return await next();
    }
}