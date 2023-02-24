using Application.Common.Behaviours;
using Application.Common.Interfaces;
using Application.UnitTests.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Common.Behaviours;

/// <summary>
///     Unit tests for the <see cref="PerformanceBehaviour{TRequest,TResponse}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class PerformanceBehaviourTests
{
    /// <summary>
    ///     The logger mock.
    /// </summary>
    private readonly Mock<ILogger<PerformanceBehaviour<TestRequest, TestResponse>>> _loggerMock;

    /// <summary>
    ///     The performance behaviour.
    /// </summary>
    private readonly PerformanceBehaviour<TestRequest, TestResponse> _performanceBehaviour;

    /// <summary>
    ///     Setups PerformanceBehaviourTests.
    /// </summary>
    public PerformanceBehaviourTests()
    {
        _loggerMock = new Mock<ILogger<PerformanceBehaviour<TestRequest, TestResponse>>>();
        Mock<ICurrentUserService> currentUserServiceMock = new();
        _performanceBehaviour =
            new PerformanceBehaviour<TestRequest, TestResponse>(_loggerMock.Object, currentUserServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method logs warning when elapsed milliseconds is greater than 500.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldLogWarning_WhenElapsedMillisecondsIsGreaterThan500()
    {
        // Arrange
        const int elapsedMilliseconds = 520;
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();
        var next = new TestRequestHandler(elapsedMilliseconds);

        // Act
        await _performanceBehaviour.Handle(request, () => next.Handle(request, cancellationToken),
            cancellationToken);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method does not log warning when elapsed milliseconds is not greater than 500.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldNotLogWarning_WhenElapsedMillisecondsIsLessThan500()
    {
        // Arrange
        const int elapsedMilliseconds = 50;
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();
        var next = new TestRequestHandler(elapsedMilliseconds);

        // Act
        await _performanceBehaviour.Handle(request, () => next.Handle(request, cancellationToken),
            cancellationToken);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    /// <summary>
    ///     Helper RequestHandler class.
    /// </summary>
    private class TestRequestHandler : IRequestHandler<TestRequest, TestResponse>
    {
        /// <summary>
        ///     The elapsed milliseconds.
        /// </summary>
        private int ElapsedMilliseconds { get; }

        /// <summary>
        ///     The response.
        /// </summary>
        private TestResponse Response { get; }

        /// <summary>
        ///     Initializes TestRequestHandler
        /// </summary>
        /// <param name="elapsedMilliseconds"></param>
        public TestRequestHandler(int elapsedMilliseconds)
        {
            ElapsedMilliseconds = elapsedMilliseconds;
            Response = new TestResponse();
        }

        /// <summary>
        ///     Handles TestRequest.
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(ElapsedMilliseconds, cancellationToken);
            return Response;
        }
    }
}