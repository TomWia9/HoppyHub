using System.Linq.Expressions;
using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Application.Users.Queries.GetUsers;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Infrastructure.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="UsersService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UsersServiceTests
{
    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The query service mock.
    /// </summary>
    private readonly Mock<IQueryService<ApplicationUser>> _queryServiceMock;

    /// <summary>
    ///     The user manager mock.
    /// </summary>
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    /// <summary>
    ///     The users service.
    /// </summary>
    private readonly IUsersService _usersService;

    /// <summary>
    ///     Setups UsersServiceTests.
    /// </summary>
    public UsersServiceTests()
    {
        _queryServiceMock = new Mock<IQueryService<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _usersService = new UsersService(_userManagerMock.Object, _queryServiceMock.Object,
            _currentUserServiceMock.Object);
    }

    /// <summary>
    ///     Tests that GetUserAsync method return UserDto when Id is valid.
    /// </summary>
    [Fact]
    public async Task GetUserAsync_ShouldReturnUserDto_WhenIdIsValid()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "user"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { Roles.User });

        // Act
        var result = await _usersService.GetUserAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.Username.Should().Be(user.UserName);
        result.Role.Should().Be(Roles.User);
    }

    /// <summary>
    ///     Tests that GetUsernameAsync method returns Username when Id is valid.
    /// </summary>
    [Fact]
    public async Task GetUsernameAsync_ShouldReturnUsername_WhenIdIsValid()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "user"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        // Act
        var result = await _usersService.GetUsernameAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(user.UserName);
    }

    /// <summary>
    ///     Tests that GetUsernameAsync method returns null when user not found.
    /// </summary>
    [Fact]
    public async Task GetUsernameAsync_ShouldReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _usersService.GetUsernameAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Tests that GetUserAsync method throws NotFound exception when Id is invalid.
    /// </summary>
    [Fact]
    public async Task GetUserAsync_ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act & Assert
        await _usersService.Invoking(x => x.GetUserAsync(Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
    }

    /// <summary>
    ///     Tests that GetUsersAsync method returns all users when request does not contain any query parameters.
    /// </summary>
    [Fact]
    public async Task GetUsersAsync_ShouldReturnAllUsers_WhenRequestDoesNotContainAnyQueryParameters()
    {
        // Arrange
        var query = new GetUsersQuery();
        var users = new List<ApplicationUser>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        }.AsQueryable();

        _userManagerMock.Setup(x => x.Users)
            .Returns(users);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { Roles.User });
        _queryServiceMock.Setup(x => x.Filter(It.IsAny<IQueryable<ApplicationUser>>(),
                It.IsAny<IEnumerable<Expression<Func<ApplicationUser, bool>>>>()))
            .Returns(users);
        _queryServiceMock.Setup(x =>
                x.Sort(It.IsAny<IQueryable<ApplicationUser>>(), It.IsAny<Expression<Func<ApplicationUser, object>>>(),
                    It.IsAny<SortDirection>()))
            .Returns(users);

        // Act
        var result = await _usersService.GetUsersAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PaginatedList<UserDto>>();
        result.Should().HaveCount(users.Count());
        result.Should().OnlyContain(u => u.GetType() == typeof(UserDto));
    }

    /// <summary>
    ///     Tests that GetUsersAsync method returns correct paginated list of UserDto when query parameters specified.
    /// </summary>
    [Fact]
    public async Task GetUsersAsync_ShouldReturnCorrectPaginatedListOfUserDto_WhenQueryParametersSpecified()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Role = Roles.User, SearchQuery = "111", SortBy = "username", SortDirection = SortDirection.Desc,
            PageNumber = 1, PageSize = 2
        };

        var users = new List<ApplicationUser>
        {
            new() { Id = Guid.NewGuid(), Email = "user1@example.com", UserName = "user1" },
            new() { Id = Guid.NewGuid(), Email = "user112@example.com", UserName = "user112" },
            new() { Id = Guid.NewGuid(), Email = "user1113@example.com", UserName = "user1113" },
            new() { Id = Guid.NewGuid(), Email = "user11114@example.com", UserName = "user11114" }
        };

        _userManagerMock.Setup(x => x.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(x => x.GetUsersInRoleAsync(query.Role)).ReturnsAsync(users.ToList());
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { Roles.User });
        _queryServiceMock.Setup(x => x.Filter(It.IsAny<IQueryable<ApplicationUser>>(),
                It.IsAny<List<Expression<Func<ApplicationUser, bool>>>>()))
            .Returns(new List<ApplicationUser> { users[2], users[3] }.AsQueryable());
        _queryServiceMock.Setup(x => x.Sort(It.IsAny<IQueryable<ApplicationUser>>(),
                It.IsAny<Expression<Func<ApplicationUser, object>>>(), It.IsAny<SortDirection>()))
            .Returns(new List<ApplicationUser> { users[3], users[2] }.AsQueryable());

        // Act
        var result = await _usersService.GetUsersAsync(query);

        // Assert
        result.Should().BeAssignableTo<PaginatedList<UserDto>>();
        result.Should().HaveCount(2);
        result[0].Username.Should().Be("user11114");
        result[1].Username.Should().Be("user1113");
    }

    /// <summary>
    ///     Tests that GetUsersAsync method returns empty list when users not found.
    /// </summary>
    [Fact]
    public async Task GetUsersAsync_ShouldReturnEmptyPaginatedList_WhenUsersNotFound()
    {
        // Arrange
        var query = new GetUsersQuery { Role = "Admin", PageNumber = 1, PageSize = 10 };
        var users = new List<ApplicationUser>();

        _userManagerMock.Setup(x => x.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(x => x.GetUsersInRoleAsync(query.Role)).ReturnsAsync(users);
        _queryServiceMock.Setup(x => x.Filter(It.IsAny<IQueryable<ApplicationUser>>(),
            It.IsAny<List<Expression<Func<ApplicationUser, bool>>>>())).Returns(users.AsQueryable());
        _queryServiceMock.Setup(x => x.Sort(It.IsAny<IQueryable<ApplicationUser>>(),
                It.IsAny<Expression<Func<ApplicationUser, object>>>(), It.IsAny<SortDirection>()))
            .Returns(users.AsQueryable());

        // Act
        var result = await _usersService.GetUsersAsync(query);

        // Assert
        result.Should().BeOfType<PaginatedList<UserDto>>();
        result.Should().HaveCount(0);
    }

    /// <summary>
    ///     Tests that UpdateUserAsync method throws NotFoundException when user not found.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var request = new UpdateUserCommand
        {
            UserId = Guid.NewGuid(),
            Username = "newUsername",
            CurrentPassword = "oldPassword",
            NewPassword = "newPassword"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var action = new Func<Task>(() => _usersService.UpdateUserAsync(request));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*{nameof(ApplicationUser)}*{request.UserId}*");
    }

    /// <summary>
    ///     Tests that UpdateUserAsync method updates username when new username provided.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUsername_WhenNewUsernameProvided()
    {
        // Arrange
        var request = new UpdateUserCommand
        {
            UserId = Guid.NewGuid(),
            Username = "newUsername"
        };

        var user = new ApplicationUser
        {
            Id = request.UserId,
            UserName = "oldUsername"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _usersService.UpdateUserAsync(request);

        // Assert
        user.UserName.Should().Be(request.Username);
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    /// <summary>
    ///     Tests that UpdateUserAsync method throws BadRequestException when update user result is not succeeded.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldThrowBadRequestException_WhenUpdateUserResultIsNotSucceeded()
    {
        // Arrange
        var request = new UpdateUserCommand
        {
            UserId = Guid.NewGuid(),
            Username = "newUsername"
        };

        var user = new ApplicationUser
        {
            Id = request.UserId,
            UserName = "oldUsername"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var action = new Func<Task>(() => _usersService.UpdateUserAsync(request));

        // Assert
        await action.Should().ThrowAsync<BadRequestException>();
    }

    /// <summary>
    ///     Tests that UpdateUserAsync method changes password when NewPassword is provided.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldChangePassword_WhenNewPasswordIsProvided()
    {
        // Arrange
        var request = new UpdateUserCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "oldPassword",
            NewPassword = "newPassword"
        };

        var user = new ApplicationUser
        {
            Id = request.UserId
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync(user);

        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _usersService.UpdateUserAsync(request);

        // Assert
        _userManagerMock.Verify(x => x.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword),
            Times.Once);
    }

    /// <summary>
    ///     Tests that UpdateUserAsync method changes password when NewPassword is provided and user is administrator.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ShouldChangePassword_WhenNewPasswordIsProvidedAndCurrentUserIsAdministrator()
    {
        // Arrange
        var request = new UpdateUserCommand
        {
            UserId = Guid.NewGuid(),
            NewPassword = "newPassword"
        };

        var user = new ApplicationUser
        {
            Id = request.UserId
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.RemovePasswordAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddPasswordAsync(user, request.NewPassword))
            .ReturnsAsync(IdentityResult.Success);
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        // Act
        await _usersService.UpdateUserAsync(request);

        // Assert
        _userManagerMock.Verify(x => x.RemovePasswordAsync(user), Times.Once);
        _userManagerMock.Verify(x => x.AddPasswordAsync(user, request.NewPassword), Times.Once);
    }

    /// <summary>
    ///     Tests that UpdateUserAsync method throws BadRequestException
    ///     when current user is administrator and RemovePassword result is not succeeded.
    /// </summary>
    [Fact]
    public async Task
        UpdateUserAsync_ShouldThrowBadRequestException_WhenCurrentUserIsAdministratorAndRemovePasswordResultIsNotSucceeded()
    {
        // Arrange
        var request = new UpdateUserCommand { UserId = Guid.NewGuid(), NewPassword = "newPassword" };
        var user = new ApplicationUser { Id = request.UserId };

        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.RemovePasswordAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError
                { Description = "Could not remove password!" }));
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        // Act
        var action = new Func<Task>(() => _usersService.UpdateUserAsync(request));

        // Assert
        await action.Should().ThrowAsync<BadRequestException>().WithMessage("Could not remove password!");
    }

    /// <summary>
    ///     Tests that UpdateUserAsync method throws BadRequestException
    ///     when current user is administrator and AddPassword result is not succeeded.
    /// </summary>
    [Fact]
    public async Task
        UpdateUserAsync_ShouldThrowBadRequestException_WhenCurrentUserIsAdministratorAndAddPasswordResultIsNotSucceeded()
    {
        // Arrange
        var request = new UpdateUserCommand { UserId = Guid.NewGuid(), NewPassword = "newPassword" };
        var user = new ApplicationUser { Id = request.UserId };

        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.RemovePasswordAsync(user)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock
            .Setup(x => x.AddPasswordAsync(user, request.NewPassword)).ReturnsAsync(IdentityResult.Failed(
                new IdentityError
                    { Description = "Could not add password!" }));
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        // Act
        var action = new Func<Task>(() => _usersService.UpdateUserAsync(request));

        // Assert
        await action.Should().ThrowAsync<BadRequestException>().WithMessage("Could not add password!");
    }

    /// <summary>
    ///     Tests that UpdateUserAsync method throws BadRequestException
    ///     when current user is administrator and AddPassword result is not succeeded.
    /// </summary>
    [Fact]
    public async Task
        UpdateUserAsync_ShouldThrowBadRequestException_WhenCurrentUserIsUserAndChangePasswordResultIsNotSucceeded()
    {
        // Arrange
        var request = new UpdateUserCommand
            { UserId = Guid.NewGuid(), CurrentPassword = "currentPassword", NewPassword = "newPassword" };
        var user = new ApplicationUser { Id = request.UserId };

        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword)).ReturnsAsync(
                IdentityResult.Failed(
                    new IdentityError
                        { Description = "Could not change password!" }));
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act
        var action = new Func<Task>(() => _usersService.UpdateUserAsync(request));

        // Assert
        await action.Should().ThrowAsync<BadRequestException>().WithMessage("Could not change password!");
    }

    /// <summary>
    ///     Tests that DeleteUserAsync method deletes user when user exists.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        const string password = "correctPassword";
        var userId = Guid.NewGuid();
        var request = new DeleteUserCommand { UserId = userId, Password = password };
        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(true);
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act
        await _usersService.DeleteUserAsync(request);

        // Assert
        _userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
    }

    /// <summary>
    ///     Tests that DeleteUserAsync method throws NotFoundException when user does not exists.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new DeleteUserCommand { UserId = userId };
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(null as ApplicationUser);

        // Act
        var act = async () => await _usersService.DeleteUserAsync(request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(ApplicationUser)}\" ({userId}) was not found.");
    }

    /// <summary>
    ///     Tests that DeleteUserAsync method throws BadRequestException when provided password is incorrect
    ///     and user is not Administrator.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldThrowBadRequestException_WhenProvidedPasswordIsIncorrectAndUserIsNotAdmin()
    {
        // Arrange
        const string password = "incorrectPassword";
        var userId = Guid.NewGuid();
        var request = new DeleteUserCommand { UserId = userId, Password = password };
        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(false);
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act
        var act = async () => await _usersService.DeleteUserAsync(request);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Provided password is incorrect");
    }

    /// <summary>
    ///     Tests that DeleteUserAsync deletes user when provided password is incorrect but user is administrator.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenProvidedPasswordIsIncorrectButUserIsAdmin()
    {
        // Arrange
        const string password = "incorrectPassword";
        var userId = Guid.NewGuid();
        var request = new DeleteUserCommand { UserId = userId, Password = password };
        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(false);
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        // Act
        await _usersService.DeleteUserAsync(request);

        // Assert
        _userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
    }
}