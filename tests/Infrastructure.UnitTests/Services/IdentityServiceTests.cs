﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Infrastructure.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="IdentityService"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class IdentityServiceTests
{
    /// <summary>
    ///     User manager mock.
    /// </summary>
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    /// <summary>
    ///     Identity Service.
    /// </summary>
    private readonly IIdentityService _identityService;

    /// <summary>
    ///     Setups IdentityServiceTests.
    /// </summary>
    public IdentityServiceTests()
    {
        var jwtSettings = new JwtSettings()
        {
            Secret = "test_secret_12345"
        };
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
        _identityService = new IdentityService(_userManagerMock.Object, jwtSettings);
    }

    /// <summary>
    ///      Tests that the RegisterAsync method returns success when new user is created.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_WithNewUser_ShouldReturnSuccess()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)(null));
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
    ///     Tests that the RegisterAsync method returns failure when user with given email already exists.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_WithExistingUser_ShouldReturnFailure()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser());

        // Act
        var result = await _identityService.RegisterAsync("test@test.com", "testuser", "password");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Token.Should().BeNullOrEmpty();
        result.Errors.Should().NotBeNullOrEmpty();
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.User), Times.Never);
    }

    /// <summary>
    ///     Tests that RegisterAsync method returns failure with correct error when creation status is failed.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_WithFailureCreationStatus_ShouldReturnFailureWithCorrectError()
    {
        // Arrange
        var identityError = new IdentityError()
        {
            Code = "test",
            Description = "test"
        };
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)(null));
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
    public async Task RegisterAsync_WhenCalledWithInvalidInput_ShouldThrowValidationException(string email,
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
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
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
    public async Task LoginAsync_WithInvalidEmail_ShouldReturnFailure()
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
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnFailure()
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
    public async Task LoginAsync_WithNoSecretInJwtSettings_ShouldReturnFailure()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@test.com" };
        var jwtSettings = new JwtSettings()
        {
            Secret = ""
        };
        var identityService = new IdentityService(_userManagerMock.Object, jwtSettings);

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
    public async Task LoginAsync_WhenCalledWithInvalidInput_ShouldThrowValidationException(string email,
        string password)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _identityService.LoginAsync(email, password));
    }

    /// <summary>
    ///     Tests that IsInRoleAsync method should return true when user is in role.
    /// </summary>
    [Fact]
    public async Task IsInRoleAsync_ShouldReturnTrue_WhenUserIsInRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var users = new List<ApplicationUser> { user };

        _userManagerMock.Setup(m => m.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, Roles.User)).ReturnsAsync(true);

        // Act
        var result = await _identityService.IsInRoleAsync(userId, Roles.User);

        // Assert
        result.Should().BeTrue();
        _userManagerMock.Verify(m => m.Users, Times.Once);
        _userManagerMock.Verify(m => m.IsInRoleAsync(user, Roles.User), Times.Once);
    }

    /// <summary>
    ///     Tests that IsInRoleAsync method should return false when user is not in role.
    /// </summary>
    [Fact]
    public async Task IsInRoleAsync_ShouldReturnFalse_WhenUserIsNotInRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var users = new List<ApplicationUser> { user };

        _userManagerMock.Setup(m => m.Users)
            .Returns(users.AsQueryable());
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, Roles.User)).ReturnsAsync(false);

        // Act
        var result = await _identityService.IsInRoleAsync(userId, Roles.User);

        // Assert
        result.Should().BeFalse();
        _userManagerMock.Verify(m => m.Users, Times.Once);
        _userManagerMock.Verify(m => m.IsInRoleAsync(user, Roles.User), Times.Once);
    }

    /// <summary>
    ///     Tests that IsInPolicyAsync method should return true when user is in policy.
    /// </summary>
    [Fact]
    public async Task IsInPolicyAsync_ShouldReturnTrue_WhenUserIsInPolicy()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var users = new List<ApplicationUser> { user };

        _userManagerMock.Setup(m => m.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, Roles.User)).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, Roles.Administrator)).ReturnsAsync(false);

        // Act
        var result = await _identityService.IsInPolicyAsync(userId, Policies.UserAccess);

        // Assert
        result.Should().BeTrue();
        _userManagerMock.Verify(m => m.Users, Times.Once);
        _userManagerMock.Verify(m => m.IsInRoleAsync(user, Roles.User), Times.Once);
    }

    /// <summary>
    ///     Tests that IsInPolicyAsync method should return false when user is not in policy.
    /// </summary>
    [Fact]
    public async Task IsInPolicyAsync_ShouldReturnFalse_WhenUserIsNotInPolicy()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var users = new List<ApplicationUser> { user };

        _userManagerMock.Setup(m => m.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, Roles.User)).ReturnsAsync(false);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, Roles.Administrator)).ReturnsAsync(false);

        // Act
        var result = await _identityService.IsInPolicyAsync(userId, Policies.UserAccess);

        // Assert
        result.Should().BeFalse();
        _userManagerMock.Verify(m => m.Users, Times.Exactly(2));
        _userManagerMock.Verify(m => m.IsInRoleAsync(user, Roles.User), Times.Once);
        _userManagerMock.Verify(m => m.IsInRoleAsync(user, Roles.Administrator), Times.Once);
    }

    /// <summary>
    ///     Tests that IsInPolicyAsync method should return true when administrator is in policy.
    /// </summary>
    [Fact]
    public async Task IsInPolicyAsync_ShouldReturnFalse_WhenAdministratorIsInPolicy()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var users = new List<ApplicationUser> { user };

        _userManagerMock.Setup(m => m.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, Roles.User)).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, Roles.Administrator)).ReturnsAsync(true);

        // Act
        var result = await _identityService.IsInPolicyAsync(userId, Policies.AdministratorAccess);

        // Assert
        result.Should().BeTrue();
        _userManagerMock.Verify(m => m.Users, Times.Once);
        _userManagerMock.Verify(m => m.IsInRoleAsync(user, Roles.Administrator), Times.Once);
    }

    /// <summary>
    ///     Tests that IsInPolicyAsync method should return false when policy does not exists.
    /// </summary>
    [Fact]
    public async Task IsInPolicyAsync_ShouldReturnFalse_WhenPolicyDoesNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _identityService.IsInPolicyAsync(userId, "nonExistentPolicy");

        // Assert
        result.Should().BeFalse();
    }
}