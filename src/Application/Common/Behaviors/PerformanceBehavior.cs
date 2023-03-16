using System.Diagnostics;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

/// <summary>
///     PerformanceBehavior class.
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    ///     The timer.
    /// </summary>
    private readonly Stopwatch _timer;

    /// <summary>
    ///     The logger.
    /// </summary>
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;


    /// <summary>
    ///     Initializes PerformanceBehavior.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="currentUserService">The current user service</param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    ///     Handles PerformanceBehavior.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">The request handler delegate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;

            _logger.LogWarning(
                "HoppyHub Long Running Request: RequestName: {Name}, ElapsedMilliseconds: {ElapsedMilliseconds}," +
                " UserId: {@UserId}, Request: {@Request}", requestName, elapsedMilliseconds, userId, request);
        }

        return response;
    }
}