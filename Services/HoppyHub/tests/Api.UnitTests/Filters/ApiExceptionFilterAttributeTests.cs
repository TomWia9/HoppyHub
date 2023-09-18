using Api.Filters;
using Application.Common.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Api.UnitTests.Filters;

/// <summary>
///     Tests for the <see cref="ApiExceptionFilterAttribute" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApiExceptionFilterAttributeTests
{
    /// <summary>
    ///     The exception context.
    /// </summary>
    private readonly ExceptionContext _exceptionContext;

    /// <summary>
    ///     The api exception filter.
    /// </summary>
    private readonly ApiExceptionFilterAttribute _filter;

    /// <summary>
    ///     Setups ApiExceptionFilterAttributeTests.
    /// </summary>
    public ApiExceptionFilterAttributeTests()
    {
        var actionContext = new ActionContext(new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        _exceptionContext = new ExceptionContext(new ActionContext(actionContext), new List<IFilterMetadata>());

        _filter = new ApiExceptionFilterAttribute();
    }

    /// <summary>
    ///     Tests that OnException method with ValidationException returns BadRequestObjectResult
    ///     with validation problem details.
    /// </summary>
    [Fact]
    public void
        OnException_ShouldReturnBadRequestObjectResultWithValidationProblemDetails_WhenValidationExceptionOccurs()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new()
            {
                PropertyName = "Name",
                ErrorMessage = "Name is required"
            }
        };
        _exceptionContext.Exception = new ValidationException(validationFailures);


        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var details = result.Value.Should().BeOfType<ValidationProblemDetails>().Subject;

        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        details.Title.Should().Be("One or more validation errors occurred.");
    }

    /// <summary>
    ///     Tests that OnException method with NotFoundException returns NotFoundObjectResult
    ///     with problem details.
    /// </summary>
    [Fact]
    public void OnException_ShouldReturnNotFoundObjectResultWithProblemDetails_WhenNotFoundExceptionOccurs()
    {
        // Arrange
        _exceptionContext.Exception = new NotFoundException("Resource not found");

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var details = result.Value.Should().BeOfType<ProblemDetails>().Subject;

        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        details.Title.Should().Be("The specified resource was not found.");
        details.Detail.Should().Be("Resource not found");
    }

    /// <summary>
    ///     Tests that OnException method with UnauthorizedAccessException returns ObjectResult
    ///     with problem details.
    /// </summary>
    [Fact]
    public void OnException_ShouldReturnObjectResultWithProblemDetails_WhenUnauthorizedAccessExceptionOccurs()
    {
        // Arrange
        _exceptionContext.Exception = new UnauthorizedAccessException("Unauthorized");

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result.Should().BeOfType<ObjectResult>().Subject;
        var details = result.Value.Should().BeOfType<ProblemDetails>().Subject;

        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        details.Title.Should().Be("Unauthorized");
    }

    /// <summary>
    ///     Tests that OnException method with ForbiddenAccessException returns ObjectResult
    ///     with problem details.
    /// </summary>
    [Fact]
    public void OnException_ShouldReturnObjectResultWithProblemDetails_WhenForbiddenAccessExceptionOccurs()
    {
        // Arrange
        _exceptionContext.Exception = new ForbiddenAccessException();

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result.Should().BeOfType<ObjectResult>().Subject;
        var details = result.Value.Should().BeOfType<ProblemDetails>().Subject;

        result.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        details.Title.Should().Be("Forbidden");
    }

    /// <summary>
    ///     Tests that OnException method with invalid ModelState returns BadRequestObjectResult.
    /// </summary>
    [Fact]
    public void OnException_ShouldReturnObjectResultWithProblemDetails_WhenModelStateIsInvalid()
    {
        // Arrange
        _exceptionContext.Exception = new Exception();
        _exceptionContext.ModelState.AddModelError("testKey", "testErrorMessage");

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result.Should().BeOfType<BadRequestObjectResult>().Subject;

        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    ///     Tests that OnException method with BadRequestException returns BadRequestObjectResult
    ///     with problem details.
    /// </summary>
    [Fact]
    public void OnException_ShouldReturnBadRequestObjectResultWithProblemDetails_WhenBadRequestExceptionOccurs()
    {
        // Arrange
        _exceptionContext.Exception = new BadRequestException();

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var details = result.Value.Should().BeOfType<ProblemDetails>().Subject;

        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        details.Title.Should().Be("Server cannot process the request.");
    }

    /// <summary>
    ///     Tests that OnException method with RemoteServiceConnectionException returns ObjectResult
    ///     with problem details and status 500.
    /// </summary>
    [Fact]
    public void
        OnException_ShouldReturnObjectResultWithProblemDetailsAndStatusCode500_WhenRemoteServiceConnectionException()
    {
        // Arrange
        const string exceptionMessage = "test message";
        _exceptionContext.Exception = new RemoteServiceConnectionException(exceptionMessage);

        // Act
        _filter.OnException(_exceptionContext);

        // Assert
        var result = _exceptionContext.Result.Should().BeOfType<ObjectResult>().Subject;
        var details = result.Value.Should().BeOfType<ProblemDetails>().Subject;

        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        details.Title.Should().Be("Cannot connect to the remote service.");
        details.Detail.Should().Be(exceptionMessage);
    }
}