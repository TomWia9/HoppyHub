using FluentValidation;
using MediatR;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Behaviours;

/// <summary>
///     ValidationBehaviour class.
/// </summary>
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    ///     The validators.
    /// </summary>
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    ///     Initializes ValidationBehaviour.
    /// </summary>
    /// <param name="validators">The validators</param>
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    ///     Handles ValidationBehaviour.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">The request handler delegate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await next();
    }
}