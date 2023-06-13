using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models.BlobContainer;
using Application.Common.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.UnitTests.Common.Services;

/// <summary>
///     Tests for the <see cref="ImagesService{T}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImagesServiceTests
{
    /// <summary>
    ///     The azure storage service mock.
    /// </summary>
    private readonly Mock<IAzureStorageService> _azureStorageServiceMock;

    /// <summary>
    ///     The opinion images service.
    /// </summary>
    private readonly IImagesService<Opinion> _opinionImagesService;

    /// <summary>
    ///     The beer images service.
    /// </summary>
    private readonly IImagesService<Beer> _beerImagesService;

    /// <summary>
    ///     Setups ImagesServiceTests.
    /// </summary>
    public ImagesServiceTests()
    {
        _azureStorageServiceMock = new Mock<IAzureStorageService>();

        _opinionImagesService = new ImagesService<Opinion>(_azureStorageServiceMock.Object);
        _beerImagesService = new ImagesService<Beer>(_azureStorageServiceMock.Object);
    }

    /// <summary>
    ///     Tests that UploadImageAsync method returns image uri when image is valid and service type is an opinion.
    /// </summary>
    [Fact]
    public async Task UploadImageAsync_ShouldReturnImageUri_WhenImageIsValidAndServiceTypeIsAnOpinion()
    {
        // Arrange
        const string expectedUri = "https://example.com/image.jpg";
        var imageMock = new Mock<IFormFile>();
        var breweryId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var opinionId = Guid.NewGuid();

        _azureStorageServiceMock
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(new BlobResponseDto
            {
                Error = false,
                Status = "ok",
                Blob = new BlobDto { Uri = expectedUri, ContentType = "test" }
            });

        // Act
        var result = await _opinionImagesService.UploadImageAsync(imageMock.Object, breweryId, beerId, opinionId);

        // Assert
        result.Should().Be(expectedUri);
    }

    /// <summary>
    ///     Tests that UploadImageAsync method returns image uri when image is valid and service type is an beer.
    /// </summary>
    [Fact]
    public async Task UploadImageAsync_ShouldReturnImageUri_WhenImageIsValidAndServiceTypeIsAnBeer()
    {
        // Arrange
        const string expectedUri = "https://example.com/image.jpg";
        var imageMock = new Mock<IFormFile>();
        var breweryId = Guid.NewGuid();
        var beerId = Guid.NewGuid();

        _azureStorageServiceMock
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(new BlobResponseDto
            {
                Error = false,
                Status = "ok",
                Blob = new BlobDto { Uri = expectedUri, ContentType = "test" }
            });

        // Act
        var result = await _beerImagesService.UploadImageAsync(imageMock.Object, breweryId, beerId);

        // Assert
        result.Should().Be(expectedUri);
    }

    /// <summary>
    ///     Tests that ImageService constructor throws KeyNotFoundException
    ///     when type of the service is other that Beer or Opinion.
    /// </summary>
    [Fact]
    public void Constructor_ShouldThrowKeyNotFoundException_WhenTypeOfTheServiceIsOtherThanBeerOrOpinion()
    {
        // Arrange
        var act = () => new ImagesService<Brewery>(_azureStorageServiceMock.Object);

        // Act & Assert
        act.Should().Throw<KeyNotFoundException>();
    }

    /// <summary>
    ///     Tests that UploadImageAsync method throws exception when upload error occurs.
    /// </summary>
    [Fact]
    public async Task UploadImageAsync_ShouldThrowException_WhenUploadErrorOccurs()
    {
        // Arrange
        var imageMock = new Mock<IFormFile>();
        var breweryId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var opinionId = Guid.NewGuid();

        _azureStorageServiceMock
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(new BlobResponseDto { Error = true });

        // Act
        var act = async () =>
            await _opinionImagesService.UploadImageAsync(imageMock.Object, breweryId, beerId, opinionId);

        // Assert
        await act.Should().ThrowAsync<RemoteServiceConnectionException>()
            .WithMessage("Failed to upload the image.");
    }

    /// <summary>
    ///     Tests that DeleteOpinionImageAsync method deletes image when image uri is valid.
    /// </summary>
    [Fact]
    public async Task DeleteImageAsync_ShouldDeleteImage_WhenImageUriIsValid()
    {
        // Arrange
        const string imageUri = "https://example.com/Opinions/image.jpg";

        _azureStorageServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(new BlobResponseDto { Error = false });

        // Act
        await _opinionImagesService.DeleteImageAsync(imageUri);

        // Assert
        _azureStorageServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    ///     Tests that DeleteImageAsync method throws exception when delete error occurs.
    /// </summary>
    [Fact]
    public async Task DeleteImageAsync_ShouldThrowException_WhenDeleteErrorOccurs()
    {
        // Arrange
        const string imageUri = "https://example.com/Opinions/image.jpg";

        _azureStorageServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(new BlobResponseDto { Error = true });

        // Act
        var act = async () => await _opinionImagesService.DeleteImageAsync(imageUri);

        // Assert
        await act.Should().ThrowAsync<RemoteServiceConnectionException>()
            .WithMessage("Failed to delete the image.");
    }

    /// <summary>
    ///     Tests that GetImageTempUri method returns image uri when image is valid.
    /// </summary>
    [Fact]
    public void GetImageTempUri_ShouldReturnTempImageUri()
    {
        // Act
        var result = _beerImagesService.GetTempImageUri();

        // Assert
        result.Should().NotBeEmpty();
    }
}