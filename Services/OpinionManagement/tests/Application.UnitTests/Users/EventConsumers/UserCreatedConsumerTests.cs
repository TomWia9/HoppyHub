using Application.Common.Interfaces;
using Application.Users.EventConsumers;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Models;

namespace Application.UnitTests.Users.EventConsumers;

/// <summary>
///     Unit tests for the <see cref="UserCreatedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserCreatedConsumerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<UserCreated>> _consumeContextMock;

    /// <summary>
    ///     The UserCreated consumer. 
    /// </summary>
    private readonly UserCreatedConsumer _consumer;

    /// <summary>
    ///     Setups UserCreatedConsumerTests.
    /// </summary>
    public UserCreatedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<UserCreated>>();
        _consumer = new UserCreatedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method adds user to database when user does not already exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldAddUserToDatabase_WhenUserDoesNotAlreadyExists()
    {
        // Arrange
        const string username = "username";
        const string role = Roles.User;
        var userId = Guid.NewGuid();
        var message = new UserCreated
        {
            Id = userId,
            Username = username,
            Role = role
        };
        var users = Enumerable.Empty<User>();
        var usersDbSetMock = users.AsQueryable().BuildMockDbSet();

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Users).Returns(usersDbSetMock.Object);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Consume method not adds user to database when user already exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldNotAddUserToDatabase_WhenUserAlreadyExists()
    {
        // Arrange
        const string username = "username";
        const string role = Roles.User;
        var userId = Guid.NewGuid();
        var message = new UserCreated
        {
            Id = userId,
            Username = username,
            Role = role
        };
        var users = new List<User> { new() { Id = userId } };
        var usersDbSetMock = users.AsQueryable().BuildMockDbSet();

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Users).Returns(usersDbSetMock.Object);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}