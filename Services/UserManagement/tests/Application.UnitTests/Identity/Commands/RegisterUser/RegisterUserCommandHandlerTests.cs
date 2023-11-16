using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Identity.Commands.RegisterUser;
using MassTransit;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Identity.Commands.RegisterUser;

/// <summary>
///     Tests for the <see cref="RegisterUserCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegisterUserCommandHandlerTests
{
    /// <summary>
    ///     The register user command handler.
    /// </summary>
    private readonly RegisterUserCommandHandler _handler;

    /// <summary>
    ///     The identity service mock.
    /// </summary>
    private readonly Mock<IIdentityService> _identityServiceMock;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     Setups RegisterUserCommandHandlerTests.
    /// </summary>
    public RegisterUserCommandHandlerTests()
    {
        _identityServiceMock = new Mock<IIdentityService>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _handler = new RegisterUserCommandHandler(_identityServiceMock.Object, _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that the Handle method calls the RegisterAsync method with the correct arguments and publishes UserCreated
    ///     event.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallRegisterAsyncAndPublishUserCreatedEvent()
    {
        // Arrange
        const string email = "test@test.com";
        const string username = "testuser";
        const string password = "testpassword";
        var userId = Guid.NewGuid();
        var authenticationResult = new AuthenticationResult(true, Array.Empty<string>(), "testToken");
        var request = new RegisterUserCommand
        {
            Email = email,
            Username = username,
            Password = password
        };
        _identityServiceMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(authenticationResult);
        _identityServiceMock.Setup(x => x.GetUserIdFromJwt(It.IsAny<string>()))
            .Returns(userId.ToString());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authenticationResult);
        _identityServiceMock.Verify(x => x.RegisterAsync(email, username, password), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<UserCreated>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that the Handle method returns an AuthenticationResult object obtained from the IdentityService.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnAuthenticationResultFromIdentityService()
    {
        // Arrange
        var expected = new AuthenticationResult(true, Array.Empty<string>(), "testToken");
        _identityServiceMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expected);

        var request = new RegisterUserCommand
        {
            Email = "test@test.com",
            Username = "testuser",
            Password = "testpassword"
        };

        // Act
        var actual = await _handler.Handle(request, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    ///     Tests that the Handle method returns an AuthenticationResult object with failed status and not publishes
    ///     UserCreated event.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnAuthenticationResultWithFailedStatusAndShouldNotPublishUserCreatedEvent()
    {
        // Arrange
        var expected = new AuthenticationResult(false, Array.Empty<string>(), "");
        _identityServiceMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expected);

        var request = new RegisterUserCommand
        {
            Email = "test@test.com",
            Username = "testuser",
            Password = "testpassword"
        };

        // Act
        var actual = await _handler.Handle(request, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(expected);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<UserCreated>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}