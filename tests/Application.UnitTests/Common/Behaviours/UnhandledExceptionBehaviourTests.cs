using Application.Common.Behaviours;
using Application.UnitTests.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Common.Behaviours;

/// <summary>
///     Unit tests for the <see cref="UnhandledExceptionBehaviour{TRequest,TResponse}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UnhandledExceptionBehaviourTests
{
    /// <summary>
    ///     The logger mock.
    /// </summary>
    private readonly Mock<ILogger<UnhandledExceptionBehaviour<TestRequest, TestResponse>>> _loggerMock;

    /// <summary>
    ///     The performance behaviour.
    /// </summary>
    private readonly UnhandledExceptionBehaviour<TestRequest, TestResponse> _unhandledExceptionBehaviour;

    /// <summary>
    ///     The request handler delegate mock.
    /// </summary>
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _requestHandlerDelegateMock;

    /// <summary>
    ///     Setups UnhandledExceptionBehaviourTests.
    /// </summary>
    public UnhandledExceptionBehaviourTests()
    {
        _loggerMock = new Mock<ILogger<UnhandledExceptionBehaviour<TestRequest, TestResponse>>>();
        _requestHandlerDelegateMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        _unhandledExceptionBehaviour = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(_loggerMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method calls next when no exception is thrown.
    /// </summary>
    [Fact]
    public async Task Handle_WhenNoExceptionIsThrown_ShouldCallNext()
    {
        // Arrange
        var request = new TestRequest();

        // Act
        await _unhandledExceptionBehaviour.Handle(request, _requestHandlerDelegateMock.Object, CancellationToken.None);

        // Assert
        _requestHandlerDelegateMock.Verify(next => next(), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle logs and throw exception when unhandled Exception is thrown.
    /// </summary>
    [Fact]
    public void Handle_WhenUnhandledExceptionIsThrown_ShouldLogAndThrow()
    {
        // Arrange
        var request = new TestRequest();
        var expectedException = new TestException();

        // Act
        Func<Task<TestResponse>> action = async () => await _unhandledExceptionBehaviour.Handle(request,
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