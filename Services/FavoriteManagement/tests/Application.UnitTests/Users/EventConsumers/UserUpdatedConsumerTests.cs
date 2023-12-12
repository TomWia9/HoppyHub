using Application.Common.Interfaces;
using Application.Users.EventConsumers;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Users.EventConsumers;

/// <summary>
///     Unit tests for the <see cref="UserUpdatedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserUpdatedConsumerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<UserUpdated>> _consumeContextMock;

    /// <summary>
    ///     The UserUpdated consumer. 
    /// </summary>
    private readonly UserUpdatedConsumer _consumer;

    /// <summary>
    ///     Setups UserUpdatedConsumerTests.
    /// </summary>
    public UserUpdatedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<UserUpdated>>();
        _consumer = new UserUpdatedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method updates user username when user exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldUpdateUserUsername_WhenUserExists()
    {
        // Arrange
        const string existingUsername = "username";
        const string updatedUsername = "updatedUsername";
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            Username = existingUsername
        };
        var message = new UserUpdated
        {
            Id = userId,
            Username = updatedUsername
        };

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(existingUser);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        existingUser.Username.Should().Be(message.Username);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Consume method not updates user username when user does not exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldNotUpdateUserUsername_WhenUserDoesNotExists()
    {
        // Arrange
        const string username = "username";
        var userId = Guid.NewGuid();
        var message = new UserUpdated
        {
            Id = userId,
            Username = username
        };

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync((User?)null);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}