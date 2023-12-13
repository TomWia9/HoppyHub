using MassTransit;
using Microsoft.Extensions.Logging;
using SharedUtilities.Behaviors;
using SharedUtilities.Exceptions;

namespace Application.Common.Behaviors;

/// <summary>
///     UnhandledExceptionBehavior class.
/// </summary>
/// <typeparam name="TRequest">The request</typeparam>
/// <typeparam name="TResponse">The response</typeparam>
public class UnhandledExceptionBehavior<TRequest, TResponse> :
    UnhandledExceptionBehaviorBase<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    ///     The list of handled exceptions.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static readonly List<Type> HandledExceptions =
    [
        typeof(ValidationException),
        typeof(NotFoundException),
        typeof(UnauthorizedAccessException),
        typeof(ForbiddenAccessException),
        typeof(BadRequestException),
        typeof(RemoteServiceConnectionException),
        typeof(RequestTimeoutException)
    ];

    /// <summary>
    ///     Initializes UnhandledExceptionBehavior.
    /// </summary>
    /// <param name="logger">The logger</param>
    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger) : base(logger,
        HandledExceptions)
    {
    }
}