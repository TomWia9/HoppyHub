using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Identity.Commands.RegisterUser;
using Moq;

namespace Application.UnitTests.Identity.Commands.RegisterUser;

/// <summary>
///     Tests for the <see cref="RegisterUserCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegisterUserCommandHandlerTests
{
    private readonly RegisterUserCommandHandler _handler;
    private readonly Mock<IIdentityService> _identityServiceMock;

    /// <summary>
    ///     Setups RegisterUserCommandHandlerTests.
    /// </summary>
    public RegisterUserCommandHandlerTests()
    {
        _identityServiceMock = new Mock<IIdentityService>();
        _handler = new RegisterUserCommandHandler(_identityServiceMock.Object);
    }

    /// <summary>
    ///     Tests that the Handle method calls the RegisterAsync method with the correct arguments.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallRegisterAsync_WithCorrectArguments()
    {
        // Arrange
        const string email = "test@test.com";
        const string username = "testuser";
        const string password = "testpassword";
        var request = new RegisterUserCommand
        {
            Email = email,
            Username = username,
            Password = password
        };

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _identityServiceMock.Verify(x => x.RegisterAsync(email, username, password), Times.Once);
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
}