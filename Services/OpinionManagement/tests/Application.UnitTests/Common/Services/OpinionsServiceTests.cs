﻿using Application.Common.Interfaces;
using Application.Common.Services;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Common.Services;

/// <summary>
///     Tests for the <see cref="OpinionsService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class OpinionsServiceTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;
    
    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    
    /// <summary>
    ///     The opinions service.
    /// </summary>
    private readonly IOpinionsService _opinionsService;
    
    /// <summary>
    ///     Setups OpinionsServiceTests.
    /// </summary>
    public OpinionsServiceTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _opinionsService = new OpinionsService(_contextMock.Object, _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that PublishOpinionChangedEventAsync method publishes BeerOpinionChanged event with correct message.
    /// </summary>
    [Fact]
    public async Task PublishOpinionChangedEventAsync_ShouldPublishBeerOpinionChangedEvent_WithCorrectMessage()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var opinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId,
            ImageUri = null
        };
        var opinions = new List<Opinion> { opinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        var expectedEventMessage = new BeerOpinionChanged
        {
            BeerId = beerId,
            NewBeerRating = Math.Round(opinions.Average(x => x.Rating), 2),
            OpinionsCount = opinions.Count
        };

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        // Act
        await _opinionsService.PublishOpinionChangedEventAsync(beerId, CancellationToken.None);

        // Assert
        _publishEndpointMock.Verify(x => x.Publish(It.Is<BeerOpinionChanged>(y =>
            y.BeerId.Equals(expectedEventMessage.BeerId) &&
            y.NewBeerRating.Equals(expectedEventMessage.NewBeerRating) &&
            y.OpinionsCount.Equals(expectedEventMessage.OpinionsCount)), It.IsAny<CancellationToken>()), Times.Once);
    }
}