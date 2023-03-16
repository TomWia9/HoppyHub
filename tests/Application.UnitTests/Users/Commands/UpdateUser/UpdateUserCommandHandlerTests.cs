using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Commands.UpdateUser;
using Moq;

namespace Application.UnitTests.Users.Commands.UpdateUser;

/// <summary>
///     Unit tests for the <see cref="UpdateUserCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateUserCommandHandlerTests
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
    private readonly UpdateUserCommandHandler _handler;

    /// <summary>
    ///     Setups UpdateUserCommandHandlerTests.
    /// </summary>
    public UpdateUserCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _usersServiceMock = new Mock<IUsersService>();
        _handler = new UpdateUserCommandHandler(_currentUserServiceMock.Object, _usersServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method throws forbidden access exception when user is not administrator
    ///     and trying to update another user.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowForbiddenAccessException_WhenUserIsNotAdminAndTryingToUpdateAnotherUser()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        var command = new UpdateUserCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenAccessException>();
        _usersServiceMock.Verify(x => x.UpdateUserAsync(It.IsAny<UpdateUserCommand>()), Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method calls UpdateUserAsync when user is administrator
    ///     and trying to update another user.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallUpdateUserAsync_WhenUserIsAdminAndTryingToUpdateAnotherUser()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        var command = new UpdateUserCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _usersServiceMock.Verify(x => x.UpdateUserAsync(command), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method calls UpdateUserAsync when user is trying
    ///     to update self.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallUpdateUserAsync_WhenUserIsTryingToUpdateSelf()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId);

        var command = new UpdateUserCommand
        {
            UserId = currentUserId
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _usersServiceMock.Verify(x => x.UpdateUserAsync(command), Times.Once);
    }
}