using System.Security.Claims;
using Api.Services;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Api.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="CurrentUserService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CurrentUserServiceTests
{
    /// <summary>
    ///     ClaimsPrincipal mock.
    /// </summary>
    private readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;

    /// <summary>
    ///     Tested service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     HttpContextAccessor mock.
    /// </summary>
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

    /// <summary>
    ///     Setups CurrentUserServiceTests.
    /// </summary>
    public CurrentUserServiceTests()
    {
        _claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        Mock<HttpContext> httpContextMock = new();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        httpContextMock.Setup(x => x.User).Returns(_claimsPrincipalMock.Object);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        _currentUserService = new CurrentUserService(_httpContextAccessorMock.Object);
    }

    /// <summary>
    ///     Tests that the UserId property returns the correct value when the HttpContext
    ///     is not null and contains an "NameIdentifier" claim.
    /// </summary>
    [Fact]
    public void UserId_ShouldReturnCorrectValue_WhenHttpContextIsNotNull()
    {
        //arrange
        var userId = Guid.NewGuid().ToString();
        var claim = new Claim(ClaimTypes.NameIdentifier, userId);
        var claimsIdentity = new ClaimsIdentity(new[] { claim });
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
        {
            User = claimsPrincipal
        });

        //act
        var result = _currentUserService.UserId;

        //assert
        result.Should().Be(userId);
    }

    /// <summary>
    ///     Tests that the UserId property returns null when the HttpContext is null.
    /// </summary>
    [Fact]
    public void UserId_ShouldReturnNull_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var result = _currentUserService.UserId;

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Tests that the UserId property returns null when the "NameIdentifier" claim is missing from the HttpContext.
    /// </summary>
    [Fact]
    public void UserId_ShouldReturnNull_WhenNameIdentifierClaimIsMissing()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.UserId;

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Tests that CurrentUserRole property returns null when http context is null.
    /// </summary>
    [Fact]
    public void CurrentUserRole_ShouldReturnNull_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var result = _currentUserService.UserRole;

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Tests that CurrentUserRole property returns user when User role is User.
    /// </summary>
    [Fact]
    public void CurrentUserRole_ShouldReturnUser_WhenUserRoleIsUser()
    {
        // Arrange
        _claimsPrincipalMock.Setup(x => x.IsInRole(Roles.User)).Returns(true);
        _claimsPrincipalMock.Setup(x => x.IsInRole(Roles.Administrator)).Returns(false);

        // Act
        var result = _currentUserService.UserRole;

        // Assert
        result.Should().Be(Roles.User);
    }

    /// <summary>
    ///     Tests that CurrentUserRole property returns Administrator when user role is Administrator.
    /// </summary>
    [Fact]
    public void CurrentUserRole_ShouldReturnAdministrator_WhenUserRoleIsAdministrator()
    {
        // Arrange
        _claimsPrincipalMock.Setup(x => x.IsInRole(Roles.User)).Returns(false);
        _claimsPrincipalMock.Setup(x => x.IsInRole(Roles.Administrator)).Returns(true);

        // Act
        var result = _currentUserService.UserRole;

        // Assert
        result.Should().Be(Roles.Administrator);
    }

    /// <summary>
    ///     Tests that AdministratorAccess property returns true when user role is Administrator.
    /// </summary>
    [Fact]
    public void AdministratorAccess_ShouldReturnTrue_WhenUserRoleIsAdministrator()
    {
        // Arrange
        _claimsPrincipalMock.Setup(x => x.IsInRole(Roles.User)).Returns(false);
        _claimsPrincipalMock.Setup(x => x.IsInRole(Roles.Administrator)).Returns(true);

        // Act
        var result = _currentUserService.AdministratorAccess;

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    ///     Tests that AdministratorAccess property returns false when user role is not Administrator.
    /// </summary>
    [Fact]
    public void AdministratorAccess_ShouldReturnFalse_WhenUserRoleIsNotAdministrator()
    {
        // Arrange
        _claimsPrincipalMock.Setup(x => x.IsInRole(Roles.User)).Returns(true);
        _claimsPrincipalMock.Setup(x => x.IsInRole(Roles.Administrator)).Returns(false);

        // Act
        var result = _currentUserService.AdministratorAccess;

        // Assert
        result.Should().BeFalse();
    }
}