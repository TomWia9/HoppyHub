using Application.Breweries.Commands.DeleteBrewery;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;

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
    ///     The ImagesDeleted request client mock.
    /// </summary>
    private readonly Mock<IRequestClient<ImagesDeleted>> _imagesDeletedRequestClientMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteBreweryCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteBreweryCommandHandlerTests.
    /// </summary>
    public DeleteBreweryCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _imagesDeletedRequestClientMock = new Mock<IRequestClient<ImagesDeleted>>();
        _handler = new DeleteBreweryCommandHandler(_contextMock.Object, _imagesDeletedRequestClientMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes brewery from database and gets ImagesDeletedFromBlobStorage response without error when brewery exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldRemoveBeerFromDatabaseAndGetImagesDeletedFromBlobStorageResponseWithoutError_WhenBeerExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var brewery = new Brewery { Id = breweryId };
        var command = new DeleteBreweryCommand { Id = breweryId };
        var imagesDeletedEvent = new ImagesDeleted
        {
            Paths = new List<string>
            {
                $"Opinions/{breweryId}",
                $"Beers/{breweryId}"
            }
        };
        var imagesDeletedFromBlobStorageResponse = new ImagesDeletedFromBlobStorage
        {
            Success = true
        };
        var responseMock = new Mock<Response<ImagesDeletedFromBlobStorage>>();

        responseMock.SetupGet(x => x.Message).Returns(imagesDeletedFromBlobStorageResponse);
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);
        _imagesDeletedRequestClientMock
            .Setup(x => x.GetResponse<ImagesDeletedFromBlobStorage>(It.IsAny<ImagesDeleted>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(responseMock.Object);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Breweries.Remove(brewery), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _imagesDeletedRequestClientMock.Verify(x => x.GetResponse<ImagesDeletedFromBlobStorage>(It.Is<ImagesDeleted>(
                y =>
                    y.Paths!.Count() == imagesDeletedEvent.Paths.Count() &&
                    y.Paths!.All(imagesDeletedEvent.Paths.Contains)), It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()), Times.Once);
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
        var breweryId = Guid.NewGuid();
        var brewery = new Brewery { Id = breweryId };
        var command = new DeleteBreweryCommand { Id = breweryId };
        var imagesDeletedFromBlobStorageResponse = new ImagesDeletedFromBlobStorage
        {
            Success = false
        };
        var responseMock = new Mock<Response<ImagesDeletedFromBlobStorage>>();

        responseMock.SetupGet(x => x.Message).Returns(imagesDeletedFromBlobStorageResponse);
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);
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
        var breweryId = Guid.NewGuid();
        var brewery = new Brewery { Id = breweryId };
        var command = new DeleteBreweryCommand { Id = breweryId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);

        _imagesDeletedRequestClientMock.Setup(x =>
                x.GetResponse<ImagesDeletedFromBlobStorage>(It.IsAny<ImagesDeleted>(), It.IsAny<CancellationToken>(),
                    It.IsAny<RequestTimeout>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}