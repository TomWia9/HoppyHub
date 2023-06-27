using Application.BeerImages.Commands.UpsertBeerImage;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     Unit tests for the <see cref="UpsertBeerImageCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpsertBeerImageCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The form file mock.
    /// </summary>
    private readonly Mock<IFormFile> _formFileMock;

    /// <summary>
    ///     The UpsertBeerImageCommand handler.
    /// </summary>
    private readonly UpsertBeerImageCommandHandler _handler;

    /// <summary>
    ///     The beers images service mock.
    /// </summary>
    private readonly Mock<IBeersImagesService> _beersImagesServiceMock;

    /// <summary>
    ///     Setups UpsertBeerImageCommandHandlerTests.
    /// </summary>
    public UpsertBeerImageCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _beersImagesServiceMock = new Mock<IBeersImagesService>();
        _formFileMock = new Mock<IFormFile>();

        _handler = new UpsertBeerImageCommandHandler(_contextMock.Object, _beersImagesServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates beer image when image does not exists and returns image uri.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateBeerImageAndReturnCorrectImageUri_WhenBeerImageDoesNotExits()
    {
        // Arrange
        const string imagePath = "Beers/image.jpg";
        const string imageUri = "https://test.blob.core.windows.net/test-container/Beers/image.jpg";
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var request = new UpsertBeerImageCommand
        {
            BeerId = beerId,
            Image = _formFileMock.Object
        };
        var beer = new Beer
        {
            Id = beerId,
            BreweryId = breweryId
        };
        var beerImages = Enumerable.Empty<BeerImage>();
        var beerImagesDbSetMock = beerImages.AsQueryable().BuildMockDbSet();

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.BeerImages).Returns(beerImagesDbSetMock.Object);
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _beersImagesServiceMock.Setup(x => x.CreateImagePath(_formFileMock.Object, breweryId, beerId))
            .Returns(imagePath);
        _beersImagesServiceMock.Setup(x => x.UploadImageAsync(imagePath, _formFileMock.Object))
            .ReturnsAsync(imageUri);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(imageUri);

        _contextMock.Verify(x => x.BeerImages.AddAsync(It.IsAny<BeerImage>(), CancellationToken.None), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method updates beer image when image exists and returns image uri.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateBeerImageAndReturnCorrectImageUri_WhenBeerImageExits()
    {
        // Arrange
        const string imagePath = "Beers/image.jpg";
        const string imageUri = "https://test.blob.core.windows.net/test-container/Beers/image.jpg";
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var request = new UpsertBeerImageCommand
        {
            BeerId = beerId,
            Image = _formFileMock.Object
        };
        var beer = new Beer
        {
            Id = beerId,
            BreweryId = breweryId
        };
        var beerImages = new List<BeerImage> { new() { BeerId = beerId } };
        var beerImagesDbSetMock = beerImages.AsQueryable().BuildMockDbSet();

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.BeerImages).Returns(beerImagesDbSetMock.Object);
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _beersImagesServiceMock.Setup(x => x.CreateImagePath(_formFileMock.Object, breweryId, beerId))
            .Returns(imagePath);
        _beersImagesServiceMock.Setup(x => x.UploadImageAsync(imagePath, _formFileMock.Object))
            .ReturnsAsync(imageUri);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(imageUri);

        _contextMock.Verify(x => x.BeerImages.AddAsync(It.IsAny<BeerImage>(), CancellationToken.None), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var command = new UpsertBeerImageCommand
        {
            BeerId = beerId,
            Image = _formFileMock.Object
        };

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Beer?)null);

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
        const string imagePath = "Beers/image.jpg";
        const string exceptionMessage = "Error occurred while uploading the image";
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var beerImages = new List<BeerImage> { new() { BeerId = beerId } };
        var beerImagesDbSetMock = beerImages.AsQueryable().BuildMockDbSet();
        var command = new UpsertBeerImageCommand
        {
            BeerId = beerId,
            Image = _formFileMock.Object
        };

        _contextMock.Setup(x => x.BeerImages).Returns(beerImagesDbSetMock.Object);
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _beersImagesServiceMock.Setup(x => x.CreateImagePath(_formFileMock.Object, breweryId, beerId))
            .Returns(imagePath);

        _beersImagesServiceMock.Setup(x => x.UploadImageAsync(imagePath, _formFileMock.Object))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}