using Application.Beers.EventConsumers;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Beers.EventConsumers;

/// <summary>
///     Tests for the <see cref="BeerCreatedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerCreatedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<BeerCreated>> _consumeContextMock;

    /// <summary>
    ///     The BeerCreated consumer.
    /// </summary>
    private readonly BeerCreatedConsumer _consumer;

    /// <summary>
    ///     The application db context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     Setups BeerCreatedConsumerTests.
    /// </summary>
    public BeerCreatedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<BeerCreated>>();

        _consumer = new BeerCreatedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method adds beer to database when message id is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldAddBeerToDatabase_WhenMessageIsValid()
    {
        // Arrange
        var beers = Enumerable.Empty<Beer>();
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var message = new BeerCreated
        {
            Id = Guid.NewGuid(),
            BreweryId = Guid.NewGuid(),
            Name = "testName"
        };
        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Beers.AddAsync(It.IsAny<Beer>(), It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}