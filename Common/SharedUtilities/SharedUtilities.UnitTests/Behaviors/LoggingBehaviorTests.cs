using Application.UnitTests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using SharedUtilities.Behaviors;
using SharedUtilities.Interfaces;

namespace SharedUtilities.UnitTests.Behaviors;

/// <summary>
///     Unit tests for the <see cref="LoggingBehavior{TRequest}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class LoggingBehaviorTests
{
    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The logger mock.
    /// </summary>
    private readonly Mock<ILogger<LoggingBehavior<TestRequest>>> _loggerMock;

    /// <summary>
    ///     The logging behavior.
    /// </summary>
    private readonly LoggingBehavior<TestRequest> _loggingBehavior;

    /// <summary>
    ///     Setups LoggingBehaviorTests.
    /// </summary>
    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest>>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _loggingBehavior = new LoggingBehavior<TestRequest>(_loggerMock.Object, _currentUserServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Process method should log request with request name and user id.
    /// </summary>
    [Fact]
    public async Task Process_ShouldLogRequestWithRequestNameAndUserId()
    {
        // Arrange
        const string requestName = nameof(TestRequest);
        var userId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var request = new TestRequest();
        var expectedLogMessage =
            $"HoppyHub request: RequestName: {requestName}, UserId: {userId}, Request: {request}";

        // Act
        await _loggingBehavior.Process(request, CancellationToken.None);

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