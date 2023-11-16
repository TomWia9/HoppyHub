﻿using Application.BeerImages.Commands.DeleteBeerImage;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     Unit tests for the <see cref="DeleteBeerImageCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBeerImageCommandHandlerTests
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
    ///     The app configuration mock.
    /// </summary>
    private readonly Mock<IAppConfiguration> _appConfigurationMock;

    /// <summary>
    ///     The DeleteBeerImageCommand handler.
    /// </summary>
    private readonly DeleteBeerImageCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteBeerImageCommandHandlerTests.
    /// </summary>
    public DeleteBeerImageCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _appConfigurationMock = new Mock<IAppConfiguration>();

        _handler = new DeleteBeerImageCommandHandler(_contextMock.Object, _publishEndpointMock.Object,
            _appConfigurationMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method publishes BeerImageDeleted event when beer and
    ///     beer image exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldPublishBeerImageDeletedEvent_WhenBeerAndBeerImageExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "test.com", TempImage = false };
        var beer = new Beer { Id = beerId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand { BeerId = beerId };
        var expectedEvent = new ImageDeleted
        {
            Uri = beer.BeerImage.ImageUri
        };

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _publishEndpointMock.Verify(x => x.Publish(It.Is<ImageDeleted>(y =>
            y.Uri == expectedEvent.Uri), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beers = Enumerable.Empty<Beer>();
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand
        {
            BeerId = beerId
        };

        _contextMock
            .Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}