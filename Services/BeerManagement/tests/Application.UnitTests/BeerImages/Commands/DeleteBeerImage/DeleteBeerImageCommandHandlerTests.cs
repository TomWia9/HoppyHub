using Application.BeerImages.Commands.DeleteBeerImage;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     Unit tests for the <see cref="DeleteBeerImageCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBeerImageCommandHandlerTests
{
    /// <summary>
    ///     The app configuration mock.
    /// </summary>
    private readonly Mock<IAppConfiguration> _appConfigurationMock;

    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The DeleteBeerImageCommand handler.
    /// </summary>
    private readonly DeleteBeerImageCommandHandler _handler;

    /// <summary>
    ///     The ImageDeleted request client.
    /// </summary>
    private readonly Mock<IRequestClient<ImageDeleted>> _imageDeletedRequestClientMock;

    /// <summary>
    ///     Setups DeleteBeerImageCommandHandlerTests.
    /// </summary>
    public DeleteBeerImageCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _imageDeletedRequestClientMock = new Mock<IRequestClient<ImageDeleted>>();
        _appConfigurationMock = new Mock<IAppConfiguration>();

        _handler = new DeleteBeerImageCommandHandler(_contextMock.Object, _imageDeletedRequestClientMock.Object,
            _appConfigurationMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method gets ImageDeletedFromBlobStorage response and change beer image to temp when beer and beer
    ///     image exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldGetImageDeletedFromBlobStorageResponseAndChangeBeerImageToTemp_WhenBeerAndBeerImageExists()
    {
        // Arrange
        const string tempImageUri = "https://test.com/temp.jpg";
        var beerId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "test.com", TempImage = false };
        var beer = new Beer { Id = beerId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand { BeerId = beerId };
        var imageDeletedEvent = new ImageDeleted
        {
            Uri = beer.BeerImage.ImageUri
        };
        var imageDeletedFromBlobStorageResponse = new ImageDeletedFromBlobStorage
        {
            Success = true
        };
        var responseMock = new Mock<Response<ImageDeletedFromBlobStorage>>();

        _appConfigurationMock.SetupGet(x => x.TempBeerImageUri).Returns(tempImageUri);
        responseMock.SetupGet(x => x.Message).Returns(imageDeletedFromBlobStorageResponse);
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _imageDeletedRequestClientMock
            .Setup(x => x.GetResponse<ImageDeletedFromBlobStorage>(imageDeletedEvent, It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(responseMock.Object);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _imageDeletedRequestClientMock.Verify(x => x.GetResponse<ImageDeletedFromBlobStorage>(It.Is<ImageDeleted>(y =>
            y.Uri == imageDeletedEvent.Uri), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()), Times.Once);
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
    ///     Tests that Handle method gets ImageDeletedFromBlobStorage response and throws
    ///     RemoteServiceConnectionException when beer and beer image exists and response has an error.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldGetImageDeletedFromBlobStorageResponseAndThrowRemoteServiceConnectionException_WhenBeerAndBeerImageExistsAndResponseHasAnError()
    {
        // Arrange
        const string expectedMessage = "There was a problem deleting the image.";
        var beerId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "test.com", TempImage = false };
        var beer = new Beer { Id = beerId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand { BeerId = beerId };
        var imageDeletedEvent = new ImageDeleted
        {
            Uri = beer.BeerImage.ImageUri
        };
        var imageDeletedFromBlobStorageResponse = new ImageDeletedFromBlobStorage
        {
            Success = false
        };
        var responseMock = new Mock<Response<ImageDeletedFromBlobStorage>>();

        responseMock.SetupGet(x => x.Message).Returns(imageDeletedFromBlobStorageResponse);
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _imageDeletedRequestClientMock
            .Setup(x => x.GetResponse<ImageDeletedFromBlobStorage>(imageDeletedEvent, It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(responseMock.Object);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<RemoteServiceConnectionException>().WithMessage(expectedMessage);

        _imageDeletedRequestClientMock.Verify(x => x.GetResponse<ImageDeletedFromBlobStorage>(It.Is<ImageDeleted>(y =>
            y.Uri == imageDeletedEvent.Uri), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method throws BadRequestException when beer image is already deleted.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenBeerImageIsAlreadyDeleted()
    {
        // Arrange
        const string expectedMessage = "Image already deleted.";
        var beerId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "temp.jpg", TempImage = true };
        var beer = new Beer { Id = beerId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand
        {
            BeerId = beerId
        };

        _contextMock
            .Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<BadRequestException>().WithMessage(expectedMessage);
    }
}