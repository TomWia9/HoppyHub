using Application.Common.Interfaces;
using Application.Common.Services;
using Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.Common.Services;

/// <summary>
///     Tests for the <see cref="OpinionsService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class OpinionsServiceTests
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
    ///     The image created request client mock.
    /// </summary>
    private readonly Mock<IRequestClient<ImageCreated>> _imageCreatedRequestClientMock;

    /// <summary>
    ///     The image deleted request client mock.
    /// </summary>
    private readonly Mock<IRequestClient<ImageDeleted>> _imageDeletedRequestClientMock;

    /// <summary>
    ///     The ImageDeleted response mock.
    /// </summary>
    private readonly Mock<Response<ImageDeletedFromBlobStorage>> _imageDeletedResponseMock;

    /// <summary>
    ///     The ImageUploaded response mock.
    /// </summary>
    private readonly Mock<Response<ImageUploaded>> _imageUploadedResponseMock;

    /// <summary>
    ///     The opinions service.
    /// </summary>
    private readonly IOpinionsService _opinionsService;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     Setups OpinionsServiceTests.
    /// </summary>
    public OpinionsServiceTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _imageCreatedRequestClientMock = new Mock<IRequestClient<ImageCreated>>();
        _imageDeletedRequestClientMock = new Mock<IRequestClient<ImageDeleted>>();
        _imageUploadedResponseMock = new Mock<Response<ImageUploaded>>();
        _imageDeletedResponseMock = new Mock<Response<ImageDeletedFromBlobStorage>>();
        _formFileMock = new Mock<IFormFile>();

        _opinionsService = new OpinionsService(_contextMock.Object, _publishEndpointMock.Object,
            _imageCreatedRequestClientMock.Object, _imageDeletedRequestClientMock.Object);
    }

    /// <summary>
    ///     Tests that UploadImageAsync method uploads image when image is not null.
    /// </summary>
    [Fact]
    public async Task UploadImageAsync_ShouldUploadImage_WhenImageIsNotNull()
    {
        // Arrange
        const string uri = "https://test.com/temp.jpg";
        var breweryId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var opinionId = Guid.NewGuid();
        var opinion = new Opinion
        {
            Id = Guid.NewGuid(),
            ImageUri = null
        };
        var imageUploadedResponse = new ImageUploaded
        {
            Uri = uri
        };

        _imageUploadedResponseMock.SetupGet(x => x.Message).Returns(imageUploadedResponse);
        _imageCreatedRequestClientMock
            .Setup(x => x.GetResponse<ImageUploaded>(It.IsAny<ImageCreated>(), It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(_imageUploadedResponseMock.Object);

        // Act
        await _opinionsService.UploadImageAsync(opinion, _formFileMock.Object, breweryId, beerId,
            opinionId, It.IsAny<CancellationToken>());

        // Assert
        opinion.ImageUri.Should().Be(uri);
        _imageCreatedRequestClientMock.Verify(
            x => x.GetResponse<ImageUploaded>(It.IsAny<ImageCreated>(), It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()),
            Times.Once);
    }

    /// <summary>
    ///     Tests that UploadImageAsync method throws RemoteServiceConnectionException when failed to upload image.
    /// </summary>
    [Fact]
    public async Task UploadImageAsync_ShouldThrowRemoteServiceConnectionException_WhenFailedToUploadImage()
    {
        // Arrange
        const string expectedMessage = "Failed to upload image.";
        var breweryId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var opinionId = Guid.NewGuid();
        var opinion = new Opinion
        {
            Id = Guid.NewGuid(),
            ImageUri = null
        };
        var imageUploadedResponse = new ImageUploaded
        {
            Uri = null
        };

        _imageUploadedResponseMock.SetupGet(x => x.Message).Returns(imageUploadedResponse);
        _imageCreatedRequestClientMock
            .Setup(x => x.GetResponse<ImageUploaded>(It.IsAny<ImageCreated>(), It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(_imageUploadedResponseMock.Object);


        // Act & Assert
        await _opinionsService.Invoking(x =>
                x.UploadImageAsync(opinion, _formFileMock.Object, breweryId, beerId, opinionId,
                    It.IsAny<CancellationToken>()))
            .Should().ThrowAsync<RemoteServiceConnectionException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that DeleteImageAsync method deletes image when image uri is not null.
    /// </summary>
    [Fact]
    public async Task DeleteImageAsync_ShouldDeleteImage_WhenImageUriIsNotNull()
    {
        // Arrange
        const string uri = "https://test.com/temp.jpg";
        var imageDeletedFromBlobStorageResponse = new ImageDeletedFromBlobStorage
        {
            Success = true
        };

        _imageDeletedResponseMock.SetupGet(x => x.Message).Returns(imageDeletedFromBlobStorageResponse);
        _imageDeletedRequestClientMock
            .Setup(x => x.GetResponse<ImageDeletedFromBlobStorage>(It.IsAny<ImageDeleted>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(_imageDeletedResponseMock.Object);

        // Act
        await _opinionsService.DeleteImageAsync(uri, It.IsAny<CancellationToken>());

        // Assert
        _imageDeletedRequestClientMock.Verify(
            x => x.GetResponse<ImageDeletedFromBlobStorage>(It.IsAny<ImageDeleted>(), It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()),
            Times.Once);
    }

    /// <summary>
    ///     Tests that DeleteImageAsync method throws RemoteServiceConnectionException when failed to delete image.
    /// </summary>
    [Fact]
    public async Task DeleteImageAsync_ShouldThrowRemoteServiceConnectionException_WhenFailedToDeleteImage()
    {
        // Arrange
        const string expectedMessage = "Failed to delete the image.";
        const string uri = "https://test.com/temp.jpg";
        var imageDeletedFromBlobStorageResponse = new ImageDeletedFromBlobStorage
        {
            Success = false
        };

        _imageDeletedResponseMock.SetupGet(x => x.Message).Returns(imageDeletedFromBlobStorageResponse);
        _imageDeletedRequestClientMock
            .Setup(x => x.GetResponse<ImageDeletedFromBlobStorage>(It.IsAny<ImageDeleted>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()))
            .ReturnsAsync(_imageDeletedResponseMock.Object);

        // Act & Assert
        await _opinionsService.Invoking(x =>
                x.DeleteImageAsync(uri, It.IsAny<CancellationToken>()))
            .Should().ThrowAsync<RemoteServiceConnectionException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that PublishOpinionChangedEventAsync method publishes BeerOpinionChanged event with correct message.
    /// </summary>
    [Fact]
    public async Task PublishOpinionChangedEventAsync_ShouldPublishBeerOpinionChangedEvent_WithCorrectMessage()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var opinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId,
            ImageUri = null
        };
        var opinions = new List<Opinion> { opinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        var expectedEventMessage = new BeerOpinionChanged
        {
            BeerId = beerId,
            NewBeerRating = Math.Round(opinions.Average(x => x.Rating), 2),
            OpinionsCount = opinions.Count
        };

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        // Act
        await _opinionsService.PublishOpinionChangedEventAsync(beerId, CancellationToken.None);

        // Assert
        _publishEndpointMock.Verify(x => x.Publish(It.Is<BeerOpinionChanged>(y =>
            y.BeerId.Equals(expectedEventMessage.BeerId) &&
            y.NewBeerRating.Equals(expectedEventMessage.NewBeerRating) &&
            y.OpinionsCount.Equals(expectedEventMessage.OpinionsCount)), It.IsAny<CancellationToken>()), Times.Once);
    }
}