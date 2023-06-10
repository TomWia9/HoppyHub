using Application.Breweries.Commands.DeleteBrewery;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Breweries.Commands.DeleteBrewery;

/// <summary>
///     Unit tests for the <see cref="DeleteBreweryCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBreweryCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The azure storage service mock.
    /// </summary>
    private readonly Mock<IAzureStorageService> _azureStorageServiceMock;

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
        _azureStorageServiceMock = new Mock<IAzureStorageService>();
        _handler = new DeleteBreweryCommandHandler(_contextMock.Object, _azureStorageServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes Brewery from database and deletes related opinion images when Brewery exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveBreweryFromDatabaseAndDeleteRelatedOpinionImages_WhenBreweryExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var brewery = new Brewery { Id = breweryId };
        var command = new DeleteBreweryCommand { Id = breweryId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);
        _azureStorageServiceMock.Setup(x => x.DeleteFilesInPath(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Breweries.Remove(brewery), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _azureStorageServiceMock.Verify(x => x.DeleteFilesInPath(It.IsAny<string>()), Times.Once);
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
        _azureStorageServiceMock.Verify(x => x.DeleteFilesInPath(It.IsAny<string>()), Times.Never);
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

        _azureStorageServiceMock.Setup(x => x.DeleteFilesInPath(It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}