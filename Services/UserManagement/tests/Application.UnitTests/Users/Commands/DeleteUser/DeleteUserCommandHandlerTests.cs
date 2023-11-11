using Application.Common.Interfaces;
using Application.Users.Commands.DeleteUser;
using MassTransit;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Users.Commands.DeleteUser;

/// <summary>
///     Unit tests for the <see cref="DeleteUserCommandHandler" /> class.
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
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

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
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new DeleteUserCommandHandler(_currentUserServiceMock.Object, _usersServiceMock.Object,
            _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method calls DeleteUserAsync method when request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallDeleteUserAsync_WhenRequestIsValid()
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
    public async Task Handle_ShouldThrowForbiddenAccessException_RequestIsInvalid()
    {
        // Arrange
        var request = new DeleteUserCommand
        {
            UserId = Guid.NewGuid()
        };
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act 
        var action = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenAccessException>();
        _usersServiceMock.Verify(x => x.DeleteUserAsync(It.IsAny<DeleteUserCommand>()), Times.Never);
    }
}