using Application.Breweries.EventConsumers;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Breweries.EventConsumers;

/// <summary>
///     Tests for the <see cref="BreweryDeletedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BreweryDeletedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<BreweryDeleted>> _consumeContextMock;

    /// <summary>
    ///     The application db context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The BeerDeleted consumer.
    /// </summary>
    private readonly BreweryDeletedConsumer _consumer;

    /// <summary>
    ///     Setups BreweryDeletedConsumerTests.
    /// </summary>
    public BreweryDeletedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<BreweryDeleted>>();

        _consumer = new BreweryDeletedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method deleted brewery related beers from database when message id is valid.
    /// </summary>
    [Fact]
    public async Task
        Consume_ShouldDeleteBeerFromDatabase_WhenMessageIsValid()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var existingBeers = new List<Beer>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BreweryId = breweryId
            },
            new()
            {
                Id = Guid.NewGuid(),
                BreweryId = breweryId
            }
        };
        var message = new BreweryDeleted
        {
            Id = breweryId
        };
        var beersDbSetMock = existingBeers.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _consumeContextMock.Setup(x => x.Message).Returns(message);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Beers.RemoveRange(It.IsAny<IQueryable<Beer>>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}