using MediatR;
using Microsoft.Extensions.Logging;
using SharedUtilities.Exceptions;

namespace SharedUtilities.Behaviors;

/// <summary>
///     UnhandledExceptionBehavior class.
/// </summary>
/// <typeparam name="TRequest">The request</typeparam>
/// <typeparam name="TResponse">The response</typeparam>
public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly List<Type> _handledExceptions = new()
    {
        typeof(ValidationException),
        typeof(NotFoundException),
        typeof(UnauthorizedAccessException),
        typeof(ForbiddenAccessException),
        typeof(BadRequestException)
    };

    /// <summary>
    ///     The logger.
    /// </summary>
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    ///     Initializes UnhandledExceptionBehavior.
    /// </summary>
    /// <param name="logger">The logger</param>
    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Handles UnhandledExceptionBehavior.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">The request handler delegate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            if (!_handledExceptions.Contains(ex.GetType()))
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogError(ex, "HoppyHub Request: Unhandled Exception ({Type}) for Request {Name} {@Request}",
                    ex.GetType(), requestName, request);
            }

            throw;
        }
    }
}