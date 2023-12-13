using Application.Common.Interfaces;
using Application.Users.EventConsumers;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Users.EventConsumers;

/// <summary>
///     Unit tests for the <see cref="UserDeletedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserDeletedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<UserDeleted>> _consumeContextMock;

    /// <summary>
    ///     The UserDeleted consumer.
    /// </summary>
    private readonly UserDeletedConsumer _consumer;

    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     Setups UserDeletedConsumerTests.
    /// </summary>
    public UserDeletedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<UserDeleted>>();
        _consumer = new UserDeletedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method deletes user when user exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            Favorites = new List<Favorite>()
        };
        var message = new UserDeleted
        {
            Id = userId
        };

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(existingUser);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Users.Remove(It.IsAny<User>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Consume method not deletes user when user does not exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldNotDeleteUserWhenUserDoesNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new UserDeleted
        {
            Id = userId
        };

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync((User?)null);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Users.Remove(It.IsAny<User>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}