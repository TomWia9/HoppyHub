using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Dtos;
using Application.Users.Queries;
using Application.Users.Queries.GetUsers;
using Moq;

namespace Application.UnitTests.Users.Queries.GetUsers;

/// <summary>
///     Unit tests for the <see cref="GetUsersQueryHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetUsersQueryHandlerTests
{
    /// <summary>
    ///     The users service mock.
    /// </summary>
    private readonly Mock<IUsersService> _usersServiceMock;

    /// <summary>
    ///     The GetUsersQueryHandler mock.
    /// </summary>
    private readonly GetUsersQueryHandler _getUsersQueryHandler;

    /// <summary>
    ///     Setups GetUsersQueryHandlerTests.
    /// </summary>
    public GetUsersQueryHandlerTests()
    {
        _usersServiceMock = new Mock<IUsersService>();
        _getUsersQueryHandler = new GetUsersQueryHandler(_usersServiceMock.Object);
    }

    /// <summary>
    ///     Handle method should return paginated list of UserDto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnPaginatedListOfUserDto()
    {
        // Arrange
        var query = new GetUsersQuery();
        var users = new List<UserDto>
        {
            new() { Id = Guid.NewGuid(), Email = "admin@test.com", Username = "Admin", Role = Roles.Administrator },
            new() { Id = Guid.NewGuid(), Email = "user1@test.com", Username = "User1", Role = Roles.User },
            new() { Id = Guid.NewGuid(), Email = "user2@test.com", Username = "User2", Role = Roles.User }
        };
        var paginatedList = PaginatedList<UserDto>.Create(users, 1, 10);

        _usersServiceMock.Setup(x => x.GetUsersAsync(It.IsAny<GetUsersQuery>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _getUsersQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(paginatedList);
    }
}