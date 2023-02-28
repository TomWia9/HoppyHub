using Api.Controllers;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Users.Queries;
using Application.Users.Queries.GetUser;
using Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="UsersController"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UsersControllerTests : ControllerSetup<UsersController>
{
    /// <summary>
    ///     Tests that GetUser endpoint returns UserDto when id is valid.
    /// </summary>
    [Fact]
    public async Task GetUser_WithValidId_ReturnsUserDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userDto = new UserDto { Id = id, Email = "user@test.com", Username = "username", Role = Roles.User };
        MediatorMock.Setup(m => m.Send(It.IsAny<GetUserQuery>(), CancellationToken.None))
            .ReturnsAsync(userDto);

        // Act
        var response = await Controller.GetUser(id);

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
        MediatorMock.Setup(m => m.Send(query, CancellationToken.None))
            .ReturnsAsync(paginatedList);

        // Act
        var response = await Controller.GetUsers(query);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(paginatedList);
        Controller.Response.Headers.Should().ContainKey("X-Pagination");
        Controller.Response.Headers["X-Pagination"].Should().BeEquivalentTo(paginatedList.GetMetadata());
    }
}