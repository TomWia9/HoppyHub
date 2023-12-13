using System.Reflection;
using Application.Common.Behaviors;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.Common.Behaviors;

/// <summary>
///     Unit tests for the <see cref="UnhandledExceptionBehavior{TRequest,TResponse}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UnhandledExceptionBehaviorTests
{
    /// <summary>
    ///     Tests that UnhandledExceptionBehavior calls base constructor with correct handled exceptions list.
    /// </summary>
    [Fact]
    public void UnhandledExceptionBehavior_ShouldCallBaseConstructor_WithCorrectHandledExceptions()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UnhandledExceptionBehavior<object, object>>>();

        var expectedHandledExceptions = new List<Type>
        {
            typeof(ValidationException),
            typeof(NotFoundException),
            typeof(UnauthorizedAccessException),
            typeof(ForbiddenAccessException),
            typeof(BadRequestException),
            typeof(RemoteServiceConnectionException),
            typeof(RequestTimeoutException)
        };

        // Act
        var behavior = new UnhandledExceptionBehavior<object, object>(loggerMock.Object);

        var handledExceptions = typeof(UnhandledExceptionBehavior<object, object>)
            .GetField("HandledExceptions",
                BindingFlags.NonPublic | BindingFlags.Static);

        var actualHandledExceptions = (List<Type>)handledExceptions!.GetValue(null)!;

        // Assert
        behavior.Should().NotBeNull();
        actualHandledExceptions.Should().BeEquivalentTo(expectedHandledExceptions);
    }
}