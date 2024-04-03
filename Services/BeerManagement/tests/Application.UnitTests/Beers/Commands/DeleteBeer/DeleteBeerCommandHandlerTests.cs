using Application.Beers.Commands.DeleteBeer;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Beers.Commands.DeleteBeer;

/// <summary>
///     Unit tests for the <see cref="DeleteBeerCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBeerCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteBeerCommandHandler _handler;

    /// <summary>
    ///     The storage container service mock.
    /// </summary>
    private readonly Mock<IStorageContainerService> _storageContainerServiceMock;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     Setups DeleteBeerCommandHandlerTests.
    /// </summary>
    public DeleteBeerCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _storageContainerServiceMock = new Mock<IStorageContainerService>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new DeleteBeerCommandHandler(_contextMock.Object, _storageContainerServiceMock.Object,
            _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes beer from database and beer image from storage container and publishes BeerDeleted event when beer exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldRemoveBeerFromDatabaseAndBeerImageFromBlobStorageAndPublishBeerDeletedEvent_WhenBeerExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer
        {
            Id = beerId, BreweryId = breweryId, Created = new DateTimeOffset(), LastModified = new DateTimeOffset(),
            CreatedBy = Guid.NewGuid(), LastModifiedBy = Guid.NewGuid()
        };
        var command = new DeleteBeerCommand { Id = beerId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Beers.Remove(beer), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _storageContainerServiceMock.Verify(x => x.DeleteFromPathAsync(It.IsAny<string>()), Times.Exactly(1));
        _publishEndpointMock.Verify(x =>
            x.Publish(It.Is<BeerDeleted>(y => y.Id == beerId), It.IsAny<CancellationToken>()));
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExist()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Beer?)null);
        var command = new DeleteBeerCommand { Id = beerId };

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
        _contextMock.Verify(x => x.Beers.Remove(It.IsAny<Beer>()), Times.Never);
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
        var beerId = Guid.NewGuid();
        var beer = new Beer { Id = beerId };
        var command = new DeleteBeerCommand { Id = beerId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);

        _storageContainerServiceMock.Setup(x => x.DeleteFromPathAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}