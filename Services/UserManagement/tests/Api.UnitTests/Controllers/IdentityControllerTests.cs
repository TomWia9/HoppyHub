using Api.Controllers;
using Application.Common.Models;
using Application.Identity.Commands.LoginUser;
using Application.Identity.Commands.RegisterUser;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="IdentityController" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class IdentityControllerTests : ControllerSetup<IdentityController>
{
    /// <summary>
    ///     Tests that Register method returns Ok when result succeeds.
    /// </summary>
    [Fact]
    public async Task Register_ShouldReturnOk_WhenResultSucceeds()
    {
        // Arrange
        var request = new RegisterUserCommand();
        var result = new AuthenticationResult(true, Array.Empty<string>(), "testToken");
        MediatorMock.Setup(x => x.Send(request, default)).ReturnsAsync(result);

        // Act
        var response = await Controller.Register(request);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(result);
    }

    /// <summary>
    ///     Tests that Register method returns BadRequest when result fails.
    /// </summary>
    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenResultFails()
    {
        // Arrange
        var request = new RegisterUserCommand();
        var result = new AuthenticationResult(false, new[] { "testError" }, string.Empty);
        MediatorMock.Setup(x => x.Send(request, default)).ReturnsAsync(result);

        // Act
        var response = await Controller.Register(request);

        // Assert
        response.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeSameAs(result);
    }

    /// <summary>
    ///     Tests that Login method returns Ok when result succeeds.
    /// </summary>
    [Fact]
    public async Task Login_ShouldReturnOk_WhenResultSucceeds()
    {
        // Arrange
        var request = new LoginUserCommand();
        var result = new AuthenticationResult(true, Array.Empty<string>(), "testToken");
        MediatorMock.Setup(x => x.Send(request, default)).ReturnsAsync(result);

        // Act
        var response = await Controller.Login(request);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(result);
    }

    /// <summary>
    ///     Tests that Login method returns BadRequest when result fails.
    /// </summary>
    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenResultFails()
    {
        // Arrange
        var request = new LoginUserCommand();
        var result = new AuthenticationResult(false, new[] { "testError" }, string.Empty);
        MediatorMock.Setup(x => x.Send(request, default)).ReturnsAsync(result);

        // Act
        var response = await Controller.Login(request);

        // Assert
        response.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeSameAs(result);
    }
}