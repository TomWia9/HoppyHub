using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Identity.Commands.LoginUser;
using Moq;

namespace Application.UnitTests.Identity.Commands.LoginUser;

/// <summary>
///     Tests for the <see cref="LoginUserCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class LoginUserCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly LoginUserCommandHandler _handler;

    /// <summary>
    ///     Setups LoginUserCommandHandlerTests.
    /// </summary>
    public LoginUserCommandHandlerTests()
    {
        _identityServiceMock = new Mock<IIdentityService>();
        _handler = new LoginUserCommandHandler(_identityServiceMock.Object);
    }

    /// <summary>
    ///     Tests that the Handle method calls the LoginAsync method with the correct arguments.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallLoginAsync_WithCorrectArguments()
    {
        // Arrange
        const string email = "test@test.com";
        const string password = "testpassword";
        var request = new LoginUserCommand
        {
            Email = email,
            Password = password
        };

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _identityServiceMock.Verify(x => x.LoginAsync(email, password), Times.Once);
    }

    /// <summary>
    ///     Tests that the Handle method returns an AuthenticationResult object obtained from the IdentityService.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnAuthenticationResultFromIdentityService()
    {
        // Arrange
        var expected = new AuthenticationResult(true, Array.Empty<string>(), "testToken");
        _identityServiceMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expected);

        var request = new LoginUserCommand
        {
            Email = "test@test.com",
            Password = "testpassword"
        };

        // Act
        var actual = await _handler.Handle(request, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}