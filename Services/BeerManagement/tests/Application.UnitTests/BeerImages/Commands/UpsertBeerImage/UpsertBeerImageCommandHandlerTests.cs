using Application.BeerImages.Commands.UpsertBeerImage;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

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
    ///     The storage container service mock.
    /// </summary>
    private readonly Mock<IStorageContainerService> _storageContainerServiceMock;

    /// <summary>
    ///     Setups UpsertBeerImageCommandHandlerTests.
    /// </summary>
    public UpsertBeerImageCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _storageContainerServiceMock = new Mock<IStorageContainerService>();
        _formFileMock = new Mock<IFormFile>();

        _handler = new UpsertBeerImageCommandHandler(_contextMock.Object, _storageContainerServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method uploads beer image and adds BeerImage to database when beer exists and beer image does not exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUploadBeerImageAndAddBeerImageToDatabase_WhenBeerExitsAndBeerImageDoesNotExists()
    {
        // Arrange
        const string imageUri = "https://test.com/test.jpg";
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
        _storageContainerServiceMock.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(imageUri);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _storageContainerServiceMock.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
        _contextMock.Verify(x => x.BeerImages.AddAsync(It.IsAny<BeerImage>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method uploads beer image and updates BeerImage when beer exists and beer image exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUploadBeerImageAndUpdateBeerImage_WhenBeerExitsAndBeerImageExists()
    {
        // Arrange
        const string imageUri = "https://test.com/test.jpg";
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
        var beerImage = new BeerImage
        {
            Id = Guid.NewGuid(),
            BeerId = beerId,
            Beer = beer,
            ImageUri = "https://test.com/oldimage.jpg",
            TempImage = false
        };
        var beerImages = new List<BeerImage> { beerImage };
        var beerImagesDbSetMock = beerImages.AsQueryable().BuildMockDbSet();

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.BeerImages).Returns(beerImagesDbSetMock.Object);
        _storageContainerServiceMock.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(imageUri);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        beerImage.ImageUri.Should().Be(imageUri);
        _storageContainerServiceMock.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
        _contextMock.Verify(x => x.BeerImages.AddAsync(It.IsAny<BeerImage>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
    ///     Tests that Handle method throws RemoteServiceConnectionException when uploaded image uri is null or empty.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_ShouldThrowRemoteServiceConnectionException_WhenUploadedImageURiIsNullOrEmpty(
        string? imageUri)
    {
        // Arrange
        const string expectedMessage = "Failed to upload image.";
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

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _storageContainerServiceMock.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(imageUri);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<RemoteServiceConnectionException>().WithMessage(expectedMessage);
    }
}