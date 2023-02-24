using Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

/// <summary>
///     LoggingBehaviour class.
/// </summary>
public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    /// <summary>
    ///     The logger.
    /// </summary>
    private readonly ILogger<LoggingBehaviour<TRequest>> _logger;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     Initializes LoggingBehaviour.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="currentUserService">The current user service</param>
    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest>> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    ///     Processes LoggingBehaviour.
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