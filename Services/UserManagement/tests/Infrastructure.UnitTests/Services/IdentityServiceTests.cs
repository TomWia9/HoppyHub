using Application.Common.Interfaces;
using Infrastructure.Identity;
using Infrastructure.UnitTests.Helpers;
using Microsoft.AspNetCore.Identity;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Models;

namespace Infrastructure.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="IdentityService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class IdentityServiceTests
{
    /// <summary>
    ///     Identity Service.
    /// </summary>
    private readonly IdentityService _identityService;

    /// <summary>
    ///     The app configuration mock.
    /// </summary>
    private readonly Mock<IAppConfiguration> _appConfigurationMock;

    /// <summary>
    ///     User manager mock.
    /// </summary>
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    /// <summary>
    ///     Setups IdentityServiceTests.
    /// </summary>
    public IdentityServiceTests()
    {
        _appConfigurationMock = new Mock<IAppConfiguration>();
        _appConfigurationMock.SetupGet(x => x.JwtSecret)
            .Returns(Enumerable.Repeat("a", 257).ToString() ?? string.Empty);
        _userManagerMock = UserManagerMockFactory.CreateUserManagerMock();
        _identityService = new IdentityService(_userManagerMock.Object, _appConfigurationMock.Object);
    }

    /// <summary>
    ///     Tests that the RegisterAsync method returns success when new user is created.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenNewUserIsCreated()
    {
        // Arrange
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { Roles.User });

        // Act
        var result = await _identityService.RegisterAsync("test@test.com", "testuser", "password");

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.Errors.Should().BeNullOrEmpty();
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.User), Times.Once);
    }

    /// <summary>
    ///     Tests that RegisterAsync method returns failure with correct error when creation status is failed.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ShouldReturnFailureWithCorrectError_WhenCreationStatusIsFailed()
    {
        // Arrange
        var identityError = new IdentityError
        {
            Code = "test",
            Description = "test"
        };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _identityService.RegisterAsync("test@test.com", "testuser", "password");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Token.Should().BeNullOrEmpty();
        result.Errors.Should().Contain(identityError.Description);
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.User), Times.Never);
    }

    /// <summary>
    ///     Tests that RegisterAsync method throws ValidationException when email,
    ///     username or password is null or whitespace.
    /// </summary>
    /// <param name="email">The email</param>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    [Theory]
    [InlineData(null, "user123", "P@ssw0rd")]
    [InlineData("user@example.com", null, "P@ssw0rd")]
    [InlineData("user@example.com", "user123", null)]
    [InlineData("", "user123", "P@ssw0rd")]
    [InlineData("user@example.com", "", "P@ssw0rd")]
    [InlineData("user@example.com", "user123", "")]
    [InlineData(" ", "user123", "P@ssw0rd")]
    [InlineData("user@example.com", " ", "P@ssw0rd")]
    [InlineData("user@example.com", "user123", " ")]
    public async Task RegisterAsync_ShouldThrowValidationException_WhenCalledWithInvalidInput(string email,
        string username,
        string password)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _identityService.RegisterAsync(email, username, password));
    }

    /// <summary>
    ///     Tests that the LoginAsync method with valid credentials returns success.
    /// </summary>
    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenGivenValidCredentials()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@test.com" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { Roles.User });

        // Act
        var result = await _identityService.LoginAsync("test@test.com", "password");

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.Errors.Should().BeNullOrEmpty();
    }

    /// <summary>
    ///     Tests that LoginAsync method returns failure when the email is invalid.
    /// </summary>
    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenEmailIsInvalid()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _identityService.LoginAsync("test@test.com", "password");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Token.Should().BeNullOrEmpty();
        result.Errors.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    ///     Tests that the LoginAsync method returns failure when the password is invalid.
    /// </summary>
    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@test.com" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _identityService.LoginAsync("test@test.com", "password");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Token.Should().BeNullOrEmpty();
        result.Errors.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    ///     Tests that LoginAsync method returns failure when JWT secret not exists.
    /// </summary>
    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenNoSecretInJwtSettings()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@test.com" };
        _appConfigurationMock.SetupGet(x => x.JwtSecret).Returns(string.Empty);

        var identityService = new IdentityService(_userManagerMock.Object, _appConfigurationMock.Object);

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await identityService.LoginAsync("test@test.com", "password");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Token.Should().BeNullOrEmpty();
        result.Errors.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    ///     Tests that LoginAsync method throws ValidationException when email or password is null or whitespace.
    /// </summary>
    /// <param name="email">The email</param>
    /// <param name="password">The password</param>
    [Theory]
    [InlineData(null, "P@ssw0rd")]
    [InlineData("user@example.com", null)]
    [InlineData("", "P@ssw0rd")]
    [InlineData("user@example.com", "")]
    [InlineData(" ", "P@ssw0rd")]
    [InlineData("user@example.com", " ")]
    public async Task LoginAsync_ShouldThrowValidationException_WhenCalledWithInvalidInput(string email,
        string password)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _identityService.LoginAsync(email, password));
    }

    /// <summary>
    ///     Tests that GetClaimValueFromJwt method returns valid claim value when token and claim type are valid.
    /// </summary>
    [Fact]
    public void GetClaimValueFromJwt_ShouldReturnClaimValue_WhenTokenAndClaimTypeAreValid()
    {
        // Arrange
        const string jwtToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        const string claimType = "sub";
        const string claimValue = "1234567890";

        // Act
        var result = _identityService.GetClaimValueFromJwt(jwtToken, claimType);

        // Assert
        result.Should().Be(claimValue);
    }

    /// <summary>
    ///     Tests that GetClaimValueFromJwt method throws argument exception when token is ivalid.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GetClaimValueFromJwt_InvalidToken_ThrowsArgumentException(string jwtToken)
    {
        // Arrange
        const string claimType = "sub";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _identityService.GetClaimValueFromJwt(jwtToken, claimType))
            .Message.Should().Contain("Invalid JWT token");
    }

    [Fact]
    public void GetClaimValueFromJwt_ClaimNotFound_ThrowsArgumentException()
    {
        // Arrange
        const string jwtToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        const string claimType = "non-existing-claim-type";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _identityService.GetClaimValueFromJwt(jwtToken, claimType))
            .Message.Should().Contain($"Claim '{claimType}' does not exist in JWT token");
    }

    /// <summary>
    ///     Tests that GetUserIdFromJwt method returns valid user id when token is valid.
    /// </summary>
    [Fact]
    public void GetUserIdFromJwt_ShouldReturnUserId_WhenTokenIsValid()
    {
        // Arrange
        const string jwtToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        const string claimValue = "1234567890";

        // Act
        var result = _identityService.GetUserIdFromJwt(jwtToken);

        // Assert
        result.Should().Be(claimValue);
    }
}