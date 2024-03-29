﻿using Application.Common.Interfaces;
using Application.Favorites.Commands.CreateFavorite;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.Favorites.Commands.CreateFavorite;

/// <summary>
///     Unit tests for the <see cref="CreateFavoriteCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateFavoriteCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly CreateFavoriteCommandHandler _handler;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     Setups CreateFavoriteCommandHandlerTests.
    /// </summary>
    public CreateFavoriteCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new CreateFavoriteCommandHandler(_contextMock.Object, _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates favorite and publishes BeerFavoritesCountChanged event.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateFavoriteAndPublishBeerFavoritesCountChangedEvent()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var request = new CreateFavoriteCommand
        {
            BeerId = beerId
        };

        var beers = new List<Beer> { new() { Id = beerId } };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var favorites = Enumerable.Empty<Favorite>();
        var favoritesDbSetMock = favorites.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _contextMock.Setup(x => x.Favorites).Returns(favoritesDbSetMock.Object);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Favorites.AddAsync(It.IsAny<Favorite>(), CancellationToken.None), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        _publishEndpointMock.Verify(x =>
            x.Publish(It.Is<BeerFavoritesCountChanged>(y => y.BeerId == beerId),
                It.IsAny<CancellationToken>()));
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundExceptionWhenBeerDoesNotExist()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var command = new CreateFavoriteCommand
        {
            BeerId = beerId
        };

        var beers = Enumerable.Empty<Beer>();
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}