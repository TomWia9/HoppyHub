using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedUtilities.Exceptions;

namespace Api.Filters;

/// <summary>
///     ApiExceptionFilterAttribute class.
/// </summary>
public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    /// <summary>
    ///     The exception handlers.
    /// </summary>
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    /// <summary>
    ///     Initializes ApiExceptionFilterAttribute.
    /// </summary>
    public ApiExceptionFilterAttribute()
    {
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(BadRequestException), HandleBadRequestException },
            { typeof(RemoteServiceConnectionException), HandleRemoteServiceConnectionException },
            { typeof(RequestTimeoutException), HandleRequestTimeoutException}
        };
    }

    /// <summary>
    ///     Overrides OnException method.
    /// </summary>
    /// <param name="context">The exception context</param>
    public override void OnException(ExceptionContext context)
    {
        HandleException(context);

        base.OnException(context);
    }

    /// <summary>
    ///     Handles exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private void HandleException(ExceptionContext context)
    {
        var type = context.Exception.GetType();

        if (_exceptionHandlers.TryGetValue(type, out var value))
        {
            value.Invoke(context);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context);
        }
    }

    /// <summary>
    ///     Handles validation exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private static void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;

        var details = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    /// <summary>
    ///     Handles not found exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private static void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message
        };

        context.Result = new NotFoundObjectResult(details);

        context.ExceptionHandled = true;
    }

    /// <summary>
    ///     Handles unauthorized access exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private static void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        context.ExceptionHandled = true;
    }

    /// <summary>
    ///     Handles forbidden access exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private static void HandleForbiddenAccessException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

        context.ExceptionHandled = true;
    }

    /// <summary>
    ///     Handles bad request exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private static void HandleBadRequestException(ExceptionContext context)
    {
        var exception = context.Exception as BadRequestException;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Server cannot process the request.",
            Detail = exception?.Message
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    ///     Handles invalid model state exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private static void HandleInvalidModelStateException(ExceptionContext context)
    {
        var details = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }
    
    /// <summary>
    ///     Handles remote service connection exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private static void HandleRemoteServiceConnectionException(ExceptionContext context)
    {
        var exception = context.Exception as RemoteServiceConnectionException;

        var details = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.4",
            Title = "Cannot connect to the remote service.",
            Detail = exception?.Message
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status503ServiceUnavailable
        };

        context.ExceptionHandled = true;
    }
    
    /// <summary>
    ///     Handles request timeout exception.
    /// </summary>
    /// <param name="context">The exception context</param>
    private static void HandleRequestTimeoutException(ExceptionContext context)
    {
        var exception = context.Exception as RequestTimeoutException;

        var details = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.5",
            Title = "Gateway Timeout",
            Detail = exception?.Message
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status504GatewayTimeout
        };

        context.ExceptionHandled = true;
    }
}