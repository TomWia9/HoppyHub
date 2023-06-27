using Application.Common.Behaviors;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Common.Behaviors;

/// <summary>
///     Unit tests for the <see cref="PerformanceBehavior{TRequest,TResponse}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class PerformanceBehaviorTests
{
    /// <summary>
    ///     The logger mock.
    /// </summary>
    private readonly Mock<ILogger<PerformanceBehavior<TestRequest, TestResponse>>> _loggerMock;

    /// <summary>
    ///     The performance behavior.
    /// </summary>
    private readonly PerformanceBehavior<TestRequest, TestResponse> _performanceBehavior;

    /// <summary>
    ///     Setups PerformanceBehaviorTests.
    /// </summary>
    public PerformanceBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<PerformanceBehavior<TestRequest, TestResponse>>>();
        Mock<ICurrentUserService> currentUserServiceMock = new();
        _performanceBehavior =
            new PerformanceBehavior<TestRequest, TestResponse>(_loggerMock.Object, currentUserServiceMock.Object);
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
        await _performanceBehavior.Handle(request, () => next.Handle(request, cancellationToken),
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
        await _performanceBehavior.Handle(request, () => next.Handle(request, cancellationToken),
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
        ///     Initializes TestRequestHandler
        /// </summary>
        /// <param name="elapsedMilliseconds"></param>
        public TestRequestHandler(int elapsedMilliseconds)
        {
            ElapsedMilliseconds = elapsedMilliseconds;
            Response = new TestResponse();
        }

        /// <summary>
        ///     The elapsed milliseconds.
        /// </summary>
        private int ElapsedMilliseconds { get; }

        /// <summary>
        ///     The response.
        /// </summary>
        private TestResponse Response { get; }

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