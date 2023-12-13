using Application.Beers.EventConsumers;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Beers.EventConsumers;

/// <summary>
///     Tests for the <see cref="BeerOpinionChangedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerOpinionChangedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<BeerOpinionChanged>> _consumeContextMock;

    /// <summary>
    ///     The BeerOpinionChanged consumer.
    /// </summary>
    private readonly BeerOpinionChangedConsumer _consumer;

    /// <summary>
    ///     The application db context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     Setups BeerOpinionChangedConsumerTests.
    /// </summary>
    public BeerOpinionChangedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<BeerOpinionChanged>>();

        _consumer = new BeerOpinionChangedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method updates beer Rating and OpinionsCount when message is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldUpdateBeerRatingAndOpinionsCount_WhenMessageIsValid()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beer = new Beer
        {
            Id = beerId,
            OpinionsCount = 0,
            Rating = 0
        };
        var message = new BeerOpinionChanged
        {
            BeerId = beerId,
            OpinionsCount = 1,
            NewBeerRating = 8
        };
        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers.FindAsync(beerId)).ReturnsAsync(beer);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        beer.OpinionsCount.Should().Be(message.OpinionsCount);
        beer.Rating.Should().Be(message.NewBeerRating);
        _contextMock.Verify(x => x.Beers.FindAsync(beerId), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}