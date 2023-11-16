using Application.Beers.Commands.DeleteBeer;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;
using SharedEvents.Responses;
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
    ///     The ImagesDeleted request client mock.
    /// </summary>
    private readonly Mock<IRequestClient<ImagesDeleted>> _imagesDeletedRequestClientMock;

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
        _imagesDeletedRequestClientMock = new Mock<IRequestClient<ImagesDeleted>>();
        _handler = new DeleteBeerCommandHandler(_contextMock.Object, _imagesDeletedRequestClientMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes beer from database and gets ImagesDeletedFromBlobStorage response without error when beer exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldRemoveBeerFromDatabaseAndGetImagesDeletedFromBlobStorageResponseWithoutError_WhenBeerExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var command = new DeleteBeerCommand { Id = beerId };
        var imagesDeletedEvent = new ImagesDeleted
        {
            Paths = new List<string>
            {
                $"Opinions/{beer.BreweryId}/{beer.Id}",
                $"Beers/{beer.BreweryId}/{beer.Id}"
            }
        };
        var imagesDeletedFromBlobStorageResponse = new ImagesDeletedFromBlobStorage
        {
            Success = true
        };
        var responseMock = new Mock<Response<ImagesDeletedFromBlobStorage>>();

        responseMock.SetupGet(x => x.Message).Returns(imagesDeletedFromBlobStorageResponse);
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _imagesDeletedRequestClientMock
            .Setup(x => x.GetResponse<ImagesDeletedFromBlobStorage>(It.IsAny<ImagesDeleted>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(responseMock.Object);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Beers.Remove(beer), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _imagesDeletedRequestClientMock.Verify(x => x.GetResponse<ImagesDeletedFromBlobStorage>(It.Is<ImagesDeleted>(
                y =>
                    y.Paths!.Count() == imagesDeletedEvent.Paths.Count() &&
                    y.Paths!.All(imagesDeletedEvent.Paths.Contains)), It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()), Times.Once);
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
        _imagesDeletedRequestClientMock.Verify(
            x => x.GetResponse<ImagesDeletedFromBlobStorage>(It.IsAny<ImagesDeleted>(), It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()),
            Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method throws RemoteServiceConnectionException when ImagesDeletedFromBlobStorage response has an error.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldThrowRemoteServiceConnectionException_WhenImagesDeletedFromBlobStorageHasAnError()
    {
        // Arrange
        const string expectedMessage = "There was a problem deleting the images.";
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var command = new DeleteBeerCommand { Id = beerId };
        var imagesDeletedFromBlobStorageResponse = new ImagesDeletedFromBlobStorage
        {
            Success = false
        };
        var responseMock = new Mock<Response<ImagesDeletedFromBlobStorage>>();

        responseMock.SetupGet(x => x.Message).Returns(imagesDeletedFromBlobStorageResponse);
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _imagesDeletedRequestClientMock
            .Setup(x => x.GetResponse<ImagesDeletedFromBlobStorage>(It.IsAny<ImagesDeleted>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(responseMock.Object);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<RemoteServiceConnectionException>().WithMessage(expectedMessage);
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

        _imagesDeletedRequestClientMock.Setup(x =>
                x.GetResponse<ImagesDeletedFromBlobStorage>(It.IsAny<ImagesDeleted>(), It.IsAny<CancellationToken>(),
                    It.IsAny<RequestTimeout>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}