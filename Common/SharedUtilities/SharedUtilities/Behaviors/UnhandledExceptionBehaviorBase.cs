using MediatR;
using Microsoft.Extensions.Logging;

namespace SharedUtilities.Behaviors;

/// <summary>
///     UnhandledExceptionBehavior base class.
/// </summary>
/// <typeparam name="TRequest">The request</typeparam>
/// <typeparam name="TResponse">The response</typeparam>
public abstract class UnhandledExceptionBehaviorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    ///     The list of handled exceptions.
    /// </summary>
    private readonly List<Type> _handledExceptions;

    /// <summary>
    ///     The logger.
    /// </summary>
    private readonly ILogger<UnhandledExceptionBehaviorBase<TRequest, TResponse>> _logger;

    /// <summary>
    ///     Initializes UnhandledExceptionBehavior.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="handledExceptions">The handled exceptions</param>
    protected UnhandledExceptionBehaviorBase(ILogger<UnhandledExceptionBehaviorBase<TRequest, TResponse>> logger,
        IEnumerable<Type> handledExceptions)
    {
        _logger = logger;
        _handledExceptions = handledExceptions.ToList();
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