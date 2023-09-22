using Application.BeerImages.Commands.DeleteBeerImage;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     Unit tests for the <see cref="DeleteBeerImageCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBeerImageCommandHandlerTests
{
    /// <summary>
    ///     The beers images service mock.
    /// </summary>
    private readonly Mock<IBeersImagesService> _beersImagesServiceMock;

    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

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
        _beersImagesServiceMock = new Mock<IBeersImagesService>();

        _handler = new DeleteBeerImageCommandHandler(_contextMock.Object, _beersImagesServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method changes beer image to temp and deletes related image from blob container when beer and
    ///     beer image exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldChangeBeerImageToTempAndDeleteRelatedImageFromBlobContainer_WhenBeerAndBeerImageExists()
    {
        // Arrange
        const string tempImageUri = "https://temp.jpg";
        var beerId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "test.com", TempImage = false };
        var beer = new Beer { Id = beerId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand { BeerId = beerId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _beersImagesServiceMock.Setup(x => x.GetTempBeerImageUri()).Returns(tempImageUri);
        _beersImagesServiceMock.Setup(x => x.DeleteImageAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _beersImagesServiceMock.Verify(x => x.DeleteImageAsync(It.IsAny<string>()), Times.Once);
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

    /// <summary>
    ///     Tests that Handle method rollbacks transaction and throws exception when error occurs.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRollbackTransactionAndThrowException_WhenErrorOccurs()
    {
        // Arrange
        const string exceptionMessage = "Error occurred while uploading the image";
        var beerId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "test.com", TempImage = false };
        var beer = new Beer { Id = beerId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand
        {
            BeerId = beerId
        };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        _beersImagesServiceMock.Setup(x => x.DeleteImageAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}