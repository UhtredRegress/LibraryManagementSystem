using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BorrowService.Application.Behavior;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validator;

    public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        IEnumerable<IValidator<TRequest>> validator)
    {
        _logger = logger;
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator.Any())
        {
            var validationContext = new ValidationContext<TRequest>(request);
            var validationResults =await Task.WhenAll(_validator.Select(x => x.ValidateAsync(validationContext, cancellationToken)));
            
            var errors = validationResults.SelectMany(x => x.Errors).Where(x => x != null).ToList();

            if (errors.Any())
            {
                _logger.LogWarning("Validation failed for {RequestType}: {Errors}", typeof(TRequest).Name, errors.Select(x => x.ErrorMessage));
                throw new ValidationException(errors);
            }
        }   
        
        return await next(cancellationToken);
    }
}