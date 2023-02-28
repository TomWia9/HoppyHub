using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Queries;
using Application.Users.Queries.GetUser;
using Moq;

namespace Application.UnitTests.Users.Queries.GetUser;

/// <summary>
///     Unit tests for the <see cref="GetUserQueryHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetUserQueryHandlerTests
{
    /// <summary>
    ///     The users service mock.
    /// </summary>
    private readonly Mock<IUsersService> _usersServiceMock;

    /// <summary>
    ///     The GetUsersQueryHandler mock.
    /// </summary>
    private readonly GetUserQueryHandler _getUsersQueryHandler;

    /// <summary>
    ///     Setups GetUserQueryHandlerTests.
    /// </summary>
    public GetUserQueryHandlerTests()
    {
        _usersServiceMock = new Mock<IUsersService>();
        _getUsersQueryHandler = new GetUserQueryHandler(_usersServiceMock.Object);
    }

    /// <summary>
    ///     Handle method should return UserDto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserQuery { UserId = userId };
        var userDto = new UserDto { Id = userId, Email = "user@test.com", Username = "User", Role = Roles.User };

        _usersServiceMock.Setup(x => x.GetUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userDto);

        // Act
        var result = await _getUsersQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDto);
    }
}