using Application.Beers.Commands.DeleteBeer;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents;
using SharedUtilities.Exceptions;

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
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteBeerCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteBeerCommandHandlerTests.
    /// </summary>
    public DeleteBeerCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new DeleteBeerCommandHandler(_contextMock.Object, _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes beer from database and publishes ImagesDeleted event when beer exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveBeerFromDatabaseAndPublishImagesDeletedEvent_WhenBeerExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var command = new DeleteBeerCommand { Id = beerId };
        var expectedEvent = new ImagesDeleted
        {
            Paths = new List<string>
            {
                $"Opinions/{beer.BreweryId}/{beer.Id}",
                $"Beers/{beer.BreweryId}/{beer.Id}"
            }
        };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Beers.Remove(beer), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(It.Is<ImagesDeleted>(y =>
            y.Paths!.Count() == expectedEvent.Paths.Count() &&
            y.Paths!.All(expectedEvent.Paths.Contains)), It.IsAny<CancellationToken>()), Times.Once);
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
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<ImagesDeleted>(), It.IsAny<CancellationToken>()),
            Times.Never);
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

        _publishEndpointMock.Setup(x => x.Publish(It.IsAny<ImagesDeleted>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}