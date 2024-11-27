using Api.Controllers;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUsername;
using Application.Users.Dtos;
using Application.Users.Queries.GetUser;
using Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedUtilities.Mappings;
using SharedUtilities.Models;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="UsersController" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UsersControllerTests : ControllerSetup<UsersController>
{
    /// <summary>
    ///     Tests that GetUser endpoint returns UserDto when id is valid.
    /// </summary>
    [Fact]
    public async Task GetUser_ShouldReturnUserDto_WhenIdIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userDto = new UserDto
        {
            Id = id, Email = "user@test.com", Username = "username", Role = Roles.User,
            Created = DateTimeOffset.UtcNow.AddYears(-2)
        };
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
    public async Task GetUsers_ShouldReturnPaginatedListOfUserDto()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new()
            {
                Id = Guid.NewGuid(), Email = "user@test.com", Username = "username", Role = Roles.User,
                Created = DateTimeOffset.UtcNow.AddYears(-5)
            },
            new()
            {
                Id = Guid.NewGuid(), Email = "user1@test.com", Username = "username1", Role = Roles.User,
                Created = DateTimeOffset.UtcNow.AddYears(-1)
            }
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

    /// <summary>
    ///     Tests that UpdateUser endpoint returns NoContent when Id is valid.
    /// </summary>
    [Fact]
    public async Task UpdateUser_ShouldReturnNoContent_WhenIdIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUsernameCommand { UserId = userId, Username = "newUsername" };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUsernameCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await Controller.UpdateUser(userId, command);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        MediatorMock.Verify(m => m.Send(It.Is<UpdateUsernameCommand>(c => c.UserId == userId), default), Times.Once);
    }

    /// <summary>
    ///     Tests that UpdateUser endpoint returns BadRequest when Id is invalid.
    /// </summary>
    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUsernameCommand { UserId = Guid.NewGuid(), Username = "newUsername" };

        // Act
        var result = await Controller.UpdateUser(userId, command);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(ExpectedInvalidIdMessage);
    }

    /// <summary>
    ///     Tests that DeleteUser endpoint returns NoContent when Id is valid.
    /// </summary>
    [Fact]
    public async Task DeleteUser_ShouldReturnNoContent_WhenIdIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deleteUserCommand = new DeleteUserCommand
        {
            UserId = userId
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await Controller.DeleteUser(userId, deleteUserCommand);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), default), Times.Once);
    }

    /// <summary>
    ///     Tests that DeleteUser endpoint returns BadRequest when Id is invalid.
    /// </summary>
    [Fact]
    public async Task DeleteUser_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { UserId = Guid.NewGuid() };

        // Act
        var result = await Controller.DeleteUser(userId, command);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(ExpectedInvalidIdMessage);
    }
}