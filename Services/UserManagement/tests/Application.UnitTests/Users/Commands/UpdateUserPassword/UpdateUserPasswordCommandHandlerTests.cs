using Application.Common.Interfaces;
using Application.Users.Commands.UpdateUserPassword;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Users.Commands.UpdateUserPassword;

/// <summary>
///     Unit tests for the <see cref="UpdateUserPasswordCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateUserPasswordCommandHandlerTests
{
    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly UpdateUserPasswordCommandHandler _handler;

    /// <summary>
    ///     The users service mock.
    /// </summary>
    private readonly Mock<IUsersService> _usersServiceMock;

    /// <summary>
    ///     Setups UpdateUserCommandHandlerTests.
    /// </summary>
    public UpdateUserPasswordCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _usersServiceMock = new Mock<IUsersService>();
        _handler = new UpdateUserPasswordCommandHandler(_currentUserServiceMock.Object, _usersServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method throws forbidden access exception when user is not administrator
    ///     and trying to update another user password.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldThrowForbiddenAccessException_WhenUserIsNotAdminAndTryingToUpdateAnotherUserPassword()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        var command = new UpdateUserPasswordCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenAccessException>();
        _usersServiceMock.Verify(x => x.ChangePasswordAsync(It.IsAny<UpdateUserPasswordCommand>()), Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method calls ChangePasswordAsync when user is administrator and trying to update another user password.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldCallChangePasswordAsync_WhenUserIsAdminAndTryingToUpdateAnotherUserPassword()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        var command = new UpdateUserPasswordCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _usersServiceMock.Verify(x => x.ChangePasswordAsync(command), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method calls ChangePasswordAsync when user is trying to update his password.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallChangePasswordAsync_WhenUserIsTryingToUpdateHisPassword()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId);

        var command = new UpdateUserPasswordCommand
        {
            UserId = currentUserId
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _usersServiceMock.Verify(x => x.ChangePasswordAsync(command), Times.Once);
    }
}