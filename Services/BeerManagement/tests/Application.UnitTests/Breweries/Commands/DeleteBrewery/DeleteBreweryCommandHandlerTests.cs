﻿using Application.Breweries.Commands.DeleteBrewery;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Breweries.Commands.DeleteBrewery;

/// <summary>
///     Unit tests for the <see cref="DeleteBreweryCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBreweryCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteBreweryCommandHandler _handler;

    /// <summary>
    ///     The storage container service mock.
    /// </summary>
    private readonly Mock<IStorageContainerService> _storageContainerServiceMock;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     Setups DeleteBreweryCommandHandlerTests.
    /// </summary>
    public DeleteBreweryCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _storageContainerServiceMock = new Mock<IStorageContainerService>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new DeleteBreweryCommandHandler(_contextMock.Object, _storageContainerServiceMock.Object,
            _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes brewery from database and removes brewery beers images fom storage container and publishes BreweryDeleted event when brewery exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldRemoveBreweryFromDatabaseAndRemoveBreweryBeersImagesFromStorageContainerAndPublishBreweryDeletedEvent_WhenBeerExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var brewery = new Brewery { Id = breweryId };
        var command = new DeleteBreweryCommand { Id = breweryId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Breweries.Remove(brewery), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _storageContainerServiceMock.Verify(x => x.DeleteFromPathAsync(It.IsAny<string>()), Times.Once);
        _publishEndpointMock.Verify(x =>
            x.Publish(It.Is<BreweryDeleted>(y => y.Id == breweryId), It.IsAny<CancellationToken>()));
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when Brewery does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBreweryDoesNotExist()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brewery?)null);
        var command = new DeleteBreweryCommand { Id = breweryId };

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
        _contextMock.Verify(x => x.Breweries.Remove(It.IsAny<Brewery>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method rollbacks transaction and throws exception when error occurs.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRollbackTransactionAndThrowException_WhenErrorOccurs()
    {
        // Arrange
        const string exceptionMessage = "Error occurred while deleting the images";
        var breweryId = Guid.NewGuid();
        var brewery = new Brewery { Id = breweryId };
        var command = new DeleteBreweryCommand { Id = breweryId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);

        _storageContainerServiceMock.Setup(x => x.DeleteFromPathAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}