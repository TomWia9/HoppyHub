using System.Reflection;
using Api.Controllers;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Users.Queries;
using Application.Users.Queries.GetUser;
using Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="UsersController"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UsersControllerTests
{
    /// <summary>
    ///     The mediator mock.
    /// </summary>
    private readonly Mock<ISender> _mediatorMock;

    /// <summary>
    ///     The users controller.
    /// </summary>
    private readonly UsersController _controller;

    /// <summary>
    ///     Setups UsersControllerTests.
    /// </summary>
    public UsersControllerTests()
    {
        _mediatorMock = new Mock<ISender>();
        var services = new ServiceCollection();

        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient(_ => _mediatorMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        _controller = new UsersController
        {
            ControllerContext = new ControllerContext
                { HttpContext = new DefaultHttpContext { RequestServices = serviceProvider } }
        };
    }

    /// <summary>
    ///     Tests that GetUser endpoint returns UserDto when id is valid.
    /// </summary>
    [Fact]
    public async Task GetUser_WithValidId_ReturnsUserDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userDto = new UserDto { Id = id, Email = "user@test.com", Username = "username", Role = Roles.User };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserQuery>(), CancellationToken.None))
            .ReturnsAsync(userDto);

        // Act
        var response = await _controller.GetUser(id);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(userDto);
    }

    /// <summary>
    ///     Tests that GetUsers endpoint returns paginated list of UserDto.
    /// </summary>
    [Fact]
    public async Task GetUsers_WithQuery_ReturnsPaginatedListOfUserDto()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new() { Id = Guid.NewGuid(), Email = "user@test.com", Username = "username", Role = Roles.User },
            new() { Id = Guid.NewGuid(), Email = "user1@test.com", Username = "username1", Role = Roles.User }
        };
        var query = new GetUsersQuery { PageNumber = 1, PageSize = 10 };
        var paginatedList = users.ToPaginatedList(query.PageNumber, query.PageSize);
        _mediatorMock.Setup(m => m.Send(query, CancellationToken.None))
            .ReturnsAsync(paginatedList);

        // Act
        var response = await _controller.GetUsers(query);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(paginatedList);
        _controller.Response.Headers.Should().ContainKey("X-Pagination");
        _controller.Response.Headers["X-Pagination"].Should().BeEquivalentTo(paginatedList.GetMetadata());
    }
}