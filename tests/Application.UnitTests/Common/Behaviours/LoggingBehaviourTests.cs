using Application.Common.Behaviours;
using Application.Common.Interfaces;
using Application.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Common.Behaviours;

/// <summary>
///     Unit tests for the <see cref="LoggingBehaviour{TRequest}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class LoggingBehaviourTests
{
    /// <summary>
    ///     The logger mock.
    /// </summary>
    private readonly Mock<ILogger<TestRequest>> _loggerMock;

    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The logging behaviour.
    /// </summary>
    private readonly LoggingBehaviour<TestRequest> _loggingBehaviour;

    /// <summary>
    ///     Setups LoggingBehaviourTests.
    /// </summary>
    public LoggingBehaviourTests()
    {
        _loggerMock = new Mock<ILogger<TestRequest>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _loggingBehaviour = new LoggingBehaviour<TestRequest>(_loggerMock.Object, _currentUserServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Process method should log request with request name and user id.
    /// </summary>
    [Fact]
    public async Task Process_ShouldLogRequest_WithRequestNameAndUserId()
    {
        // Arrange
        const string requestName = nameof(TestRequest);
        var userId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var request = new TestRequest();
        var expectedLogMessage =
            $"HoppyHub request: RequestName: {requestName}, UserId: {userId}, Request: {request}";

        // Act
        await _loggingBehaviour.Process(request, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    string.Equals(expectedLogMessage, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}