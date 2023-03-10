using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Commands.DeleteUser;
using Moq;

namespace Application.UnitTests.Users.Commands.DeleteUser;

/// <summary>
///     Unit tests for the <see cref="DeleteUserCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteUserCommandHandlerTests
{
    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The users service mock.
    /// </summary>
    private readonly Mock<IUsersService> _usersServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteUserCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteUserCommandHandlerTests.
    /// </summary>
    public DeleteUserCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _usersServiceMock = new Mock<IUsersService>();
        _handler = new DeleteUserCommandHandler(_currentUserServiceMock.Object, _usersServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method calls DeleteUserAsync method when request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_WithValidRequest_ShouldCallDeleteUserAsync()
    {
        // Arrange
        var request = new DeleteUserCommand
        {
            UserId = Guid.NewGuid()
        };
        _currentUserServiceMock.Setup(x => x.UserId).Returns(request.UserId);
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _usersServiceMock.Verify(x => x.DeleteUserAsync(request), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws ForbiddenAccessException when request is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldThrowForbiddenAccessException()
    {
        // Arrange
        var request = new DeleteUserCommand
        {
            UserId = Guid.NewGuid()
        };
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act 
        Func<Task> action = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenAccessException>();
        _usersServiceMock.Verify(x => x.DeleteUserAsync(It.IsAny<DeleteUserCommand>()), Times.Never);
    }
}