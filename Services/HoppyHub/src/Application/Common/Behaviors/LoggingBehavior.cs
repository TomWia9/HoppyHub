using Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

/// <summary>
///     LoggingBehavior class.
/// </summary>
public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The logger.
    /// </summary>
    private readonly ILogger<LoggingBehavior<TRequest>> _logger;

    /// <summary>
    ///     Initializes LoggingBehavior.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="currentUserService">The current user service</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest>> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    ///     Processes LoggingBehavior.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId;

        _logger.LogInformation("HoppyHub request: RequestName: {Name}, UserId: {@UserId}, Request: {@Request}",
            requestName, userId, request);
        return Task.CompletedTask;
    }
}