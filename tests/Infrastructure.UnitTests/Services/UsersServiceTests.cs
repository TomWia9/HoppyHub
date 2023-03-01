using System.Linq.Expressions;
using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Queries;
using Application.Users.Queries.GetUsers;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Infrastructure.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="UsersService"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UsersServiceTests
{
    /// <summary>
    ///     The user manager mock.
    /// </summary>
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    /// <summary>
    ///     The query service mock.
    /// </summary>
    private readonly Mock<IQueryService<ApplicationUser>> _queryServiceMock;

    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

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
    ///     Tests that GetUserAsync method return UserDto when UserId is valid.
    /// </summary>
    [Fact]
    public async Task GetUserAsync_WithValidUserId_ReturnsUserDto()
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
    ///     Tests that GetUSerAsync method throws NotFound exception when UserId is invalid.
    /// </summary>
    [Fact]
    public async Task GetUserAsync_WithInvalidUserId_ThrowsNotFoundException()
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
    public async Task GetUsersAsync_WithoutQueryParameters_ReturnsAllUsers()
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
    public async Task GetUsersAsync_ReturnsCorrectPaginatedListOfUserDto_WhenQueryParametersSpecified()
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
    public async Task GetUsersAsync_ReturnsEmptyPaginatedList_WhenUsersNotFound()
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
}