using Application.BeerImages.Commands.UpsertBeerImage;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;

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
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IRequestClient<ImageCreated>> _imageCreatedRequestClientMock;

    /// <summary>
    ///     The form file mock.
    /// </summary>
    private readonly Mock<IFormFile> _formFileMock;

    /// <summary>
    ///     The UpsertBeerImageCommand handler.
    /// </summary>
    private readonly UpsertBeerImageCommandHandler _handler;

    /// <summary>
    ///     Setups UpsertBeerImageCommandHandlerTests.
    /// </summary>
    public UpsertBeerImageCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _imageCreatedRequestClientMock = new Mock<IRequestClient<ImageCreated>>();
        _formFileMock = new Mock<IFormFile>();

        _handler = new UpsertBeerImageCommandHandler(_contextMock.Object, _imageCreatedRequestClientMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method gets ImageUploaded response and add BeerImage to database when beer exists and beer image does not exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldGetImageUploadedResponseAndAddBeerImageToDatabase_WhenBeerExitsAndBeerImageDoesNotExists()
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
        var imageCreatedEvent = new ImageCreated
        {
            Path = $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}",
            Image = await request.Image.GetBytes()
        };
        var imageUploadedEvent = new ImageUploaded
        {
            Uri = imageUri
        };
        var responseMock = new Mock<Response<ImageUploaded>>();

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.BeerImages).Returns(beerImagesDbSetMock.Object);
        responseMock.SetupGet(x => x.Message).Returns(imageUploadedEvent);
        _imageCreatedRequestClientMock
            .Setup(x => x.GetResponse<ImageUploaded>(imageCreatedEvent, It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(responseMock.Object);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _imageCreatedRequestClientMock.Verify(x => x.GetResponse<ImageUploaded>(It.Is<ImageCreated>(y =>
                y.Path == imageCreatedEvent.Path && y.Image == imageCreatedEvent.Image),
            It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()), Times.Once);
        _contextMock.Verify(x => x.BeerImages.AddAsync(It.IsAny<BeerImage>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method gets ImageUploaded response and updates BeerImage when beer exists and beer image exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldGetImageUploadedResponseAndUpdatesBeerImage_WhenBeerExitsAndBeerImageExists()
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
            BeerId = beerId
        };
        var beerImages = new List<BeerImage> { beerImage };
        var beerImagesDbSetMock = beerImages.AsQueryable().BuildMockDbSet();
        var imageCreatedEvent = new ImageCreated
        {
            Path = $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}",
            Image = await request.Image.GetBytes()
        };
        var imageUploadedEvent = new ImageUploaded
        {
            Uri = imageUri
        };
        var responseMock = new Mock<Response<ImageUploaded>>();

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.BeerImages).Returns(beerImagesDbSetMock.Object);
        responseMock.SetupGet(x => x.Message).Returns(imageUploadedEvent);
        _imageCreatedRequestClientMock
            .Setup(x => x.GetResponse<ImageUploaded>(imageCreatedEvent, It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(responseMock.Object);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _imageCreatedRequestClientMock.Verify(x => x.GetResponse<ImageUploaded>(It.Is<ImageCreated>(y =>
                y.Path == imageCreatedEvent.Path && y.Image == imageCreatedEvent.Image),
            It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()), Times.Once);
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
        string imageUri)
    {
        // Arrange
        const string expectedMessage = "Failed to sent image.";
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
        var imageCreatedEvent = new ImageCreated
        {
            Path = $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}",
            Image = await request.Image.GetBytes()
        };
        var imageUploadedEvent = new ImageUploaded
        {
            Uri = imageUri
        };
        var responseMock = new Mock<Response<ImageUploaded>>();

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        responseMock.SetupGet(x => x.Message).Returns(imageUploadedEvent);
        _imageCreatedRequestClientMock
            .Setup(x => x.GetResponse<ImageUploaded>(imageCreatedEvent, It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(responseMock.Object);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<RemoteServiceConnectionException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that Handle method throws RequestTimeoutException when response from ImageUploaded response take too long.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowRequestTimeoutException_WhenResponseFromImageUploadedTakeTooLong()
    {
        // Arrange

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
        var imageCreatedEvent = new ImageCreated
        {
            Path = $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}",
            Image = await request.Image.GetBytes()
        };

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _imageCreatedRequestClientMock
            .Setup(x => x.GetResponse<ImageUploaded>(imageCreatedEvent, It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ThrowsAsync(new RequestTimeoutException());

        // Act & Assert
        await _handler.Invoking(x => x.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<RequestTimeoutException>();
    }
}