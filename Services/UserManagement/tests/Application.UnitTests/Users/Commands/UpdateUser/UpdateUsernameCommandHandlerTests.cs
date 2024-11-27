using Application.Common.Interfaces;
using Application.Users.Commands.UpdateUsername;
using MassTransit;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Users.Commands.UpdateUser;

/// <summary>
///     Unit tests for the <see cref="UpdateUsernameCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateUsernameCommandHandlerTests
{
    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly UpdateUsernameCommandHandler _handler;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     The users service mock.
    /// </summary>
    private readonly Mock<IUsersService> _usersServiceMock;

    /// <summary>
    ///     Setups UpdateUserCommandHandlerTests.
    /// </summary>
    public UpdateUsernameCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _usersServiceMock = new Mock<IUsersService>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new UpdateUsernameCommandHandler(_currentUserServiceMock.Object, _usersServiceMock.Object,
            _publishEndpointMock.Object);
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

        var command = new UpdateUsernameCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenAccessException>();
        _usersServiceMock.Verify(x => x.UpdateUserAsync(It.IsAny<UpdateUsernameCommand>()), Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method calls UpdateUserAsync and publishes UserUpdated event when user is administrator
    ///     and trying to update another user.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldCallUpdateUserAsyncAndPublishUserUpdatedEvent_WhenUserIsAdminAndTryingToUpdateAnotherUser()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        var command = new UpdateUsernameCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _usersServiceMock.Verify(x => x.UpdateUserAsync(command), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<UserUpdated>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method calls UpdateUserAsync and publishes UserUpdated event when user is trying
    ///     to update self.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallUpdateUserAsyncAndPublishUserUpdatedEvent_WhenUserIsTryingToUpdateSelf()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId);

        var command = new UpdateUsernameCommand
        {
            UserId = currentUserId
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _usersServiceMock.Verify(x => x.UpdateUserAsync(command), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<UserUpdated>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}