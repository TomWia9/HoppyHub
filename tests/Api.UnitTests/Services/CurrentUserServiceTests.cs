using System.Security.Claims;
using Api.Services;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Api.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="CurrentUserService"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CurrentUserServiceTests
{
    /// <summary>
    ///     HttpContextAccessor mock.
    /// </summary>
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    
    /// <summary>
    ///     Tested service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    public CurrentUserServiceTests()
    {
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _currentUserService = new CurrentUserService(_httpContextAccessor.Object);
    }

    /// <summary>
    ///     Tests that the UserId property returns the correct value when the HttpContext
    ///     is not null and contains an "id" claim.
    /// </summary>
    [Fact]
    public void UserId_ReturnsCorrectValue_WhenHttpContextIsNotNull()
    {
        //arrange
        var userId = Guid.NewGuid().ToString();
        var claim = new Claim("id", userId);
        var claimsIdentity = new ClaimsIdentity(new[] { claim });
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _httpContextAccessor.Setup(x => x.HttpContext)
            .Returns(httpContext);
        
        //act
        var result = _currentUserService.UserId;

        //assert
        result.Should().Be(userId);
    }
    
    /// <summary>
    ///     Tests that the UserId property returns null when the HttpContext is null.
    /// </summary>
    [Fact]
    public void UserId_ReturnsNull_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var result = _currentUserService.UserId;

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Tests that the UserId property returns null when the "id" claim is missing from the HttpContext.
    /// </summary>
    [Fact]
    public void UserId_ReturnsNull_WhenIdClaimIsMissing()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.UserId;

        // Assert
        result.Should().BeNull();
    }
}