using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

/// <summary>
///     UnhandledExceptionBehaviour class.
/// </summary>
/// <typeparam name="TRequest">The request</typeparam>
/// <typeparam name="TResponse">The response</typeparam>
public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    ///     The logger
    /// </summary>
    private readonly ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> _logger;

    /// <summary>
    ///     Initializes UnhandledExceptionBehaviour.
    /// </summary>
    /// <param name="logger">The logger</param>
    public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Handles UnhandledExceptionBehaviour.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">The next</param>
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
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "HoppyHub Request: Unhandled Exception for Request {Name} {@Request}",
                requestName, request);

            throw;
        }
    }
}