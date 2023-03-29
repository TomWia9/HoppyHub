using Application.Common.Behaviors;
using Application.UnitTests.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Common.Behaviors;

/// <summary>
///     Unit tests for the <see cref="UnhandledExceptionBehavior{TRequest,TResponse}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UnhandledExceptionBehaviorTests
{
    /// <summary>
    ///     The logger mock.
    /// </summary>
    private readonly Mock<ILogger<UnhandledExceptionBehavior<TestRequest, TestResponse>>> _loggerMock;

    /// <summary>
    ///     The performance behavior.
    /// </summary>
    private readonly UnhandledExceptionBehavior<TestRequest, TestResponse> _unhandledExceptionBehavior;

    /// <summary>
    ///     The request handler delegate mock.
    /// </summary>
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _requestHandlerDelegateMock;

    /// <summary>
    ///     Setups UnhandledExceptionBehaviorTests.
    /// </summary>
    public UnhandledExceptionBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<UnhandledExceptionBehavior<TestRequest, TestResponse>>>();
        _requestHandlerDelegateMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        _unhandledExceptionBehavior = new UnhandledExceptionBehavior<TestRequest, TestResponse>(_loggerMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method calls next when no exception is thrown.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoExceptionIsThrown()
    {
        // Arrange
        var request = new TestRequest();

        // Act
        await _unhandledExceptionBehavior.Handle(request, _requestHandlerDelegateMock.Object, CancellationToken.None);

        // Assert
        _requestHandlerDelegateMock.Verify(next => next(), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle logs and throw exception when unhandled Exception is thrown.
    /// </summary>
    [Fact]
    public void Handle_ShouldLogAndThrow_WhenUnhandledExceptionIsThrown()
    {
        // Arrange
        var request = new TestRequest();
        var expectedException = new TestException();

        // Act
        Func<Task<TestResponse>> action = async () => await _unhandledExceptionBehavior.Handle(request,
            () => throw expectedException, CancellationToken.None);

        // Assert
        action.Should().ThrowExactlyAsync<TestException>();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private class TestException : Exception
    {
    }
}