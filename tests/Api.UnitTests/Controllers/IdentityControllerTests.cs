using System.Reflection;
using Api.Controllers;
using Application.Common.Models;
using Application.Identity.Commands.LoginUser;
using Application.Identity.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="IdentityController"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class IdentityControllerTests
{
    /// <summary>
    ///     The mediator mock.
    /// </summary>
    private readonly Mock<ISender> _mediatorMock;

    /// <summary>
    ///     The identity controller.
    /// </summary>
    private readonly IdentityController _controller;

    /// <summary>
    ///     Setups IdentityControllerTests.
    /// </summary>
    public IdentityControllerTests()
    {
        _mediatorMock = new Mock<ISender>();
        var services = new ServiceCollection();

        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient(_ => _mediatorMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        _controller = new IdentityController
        {
            ControllerContext = new ControllerContext
                { HttpContext = new DefaultHttpContext { RequestServices = serviceProvider } }
        };
    }

    /// <summary>
    ///     Tests that Register method returns Ok when result succeeds.
    /// </summary>
    [Fact]
    public async Task Register_Returns_Ok_When_Result_Succeeds()
    {
        // Arrange
        var request = new RegisterUserCommand();
        var result = new AuthenticationResult(true, Array.Empty<string>(), "testToken");
        _mediatorMock.Setup(x => x.Send(request, default)).ReturnsAsync(result);

        // Act
        var response = await _controller.Register(request);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(result);
    }

    /// <summary>
    ///     Tests that Register method returns BadRequest when result fails.
    /// </summary>
    [Fact]
    public async Task Register_Returns_BadRequest_When_Result_Fails()
    {
        // Arrange
        var request = new RegisterUserCommand();
        var result = new AuthenticationResult(false, new[] { "testError" }, string.Empty);
        _mediatorMock.Setup(x => x.Send(request, default)).ReturnsAsync(result);

        // Act
        var response = await _controller.Register(request);

        // Assert
        response.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeSameAs(result);
    }

    /// <summary>
    ///     Tests that Login method returns Ok when result succeeds.
    /// </summary>
    [Fact]
    public async Task Login_Returns_Ok_When_Result_Succeeds()
    {
        // Arrange
        var request = new LoginUserCommand();
        var result = new AuthenticationResult(true, Array.Empty<string>(), "testToken");
        _mediatorMock.Setup(x => x.Send(request, default)).ReturnsAsync(result);

        // Act
        var response = await _controller.Login(request);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(result);
    }

    /// <summary>
    ///     Tests that Login method returns BadRequest when result fails.
    /// </summary>
    [Fact]
    public async Task Login_Returns_BadRequest_When_Result_Fails()
    {
        // Arrange
        var request = new LoginUserCommand();
        var result = new AuthenticationResult(false, new[] { "testError" }, string.Empty);
        _mediatorMock.Setup(x => x.Send(request, default)).ReturnsAsync(result);

        // Act
        var response = await _controller.Login(request);

        // Assert
        response.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeSameAs(result);
    }
}