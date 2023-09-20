using Application.Common.Interfaces;
using Application.Common.Models.BlobContainer;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using SharedUtilities.Exceptions;

namespace Infrastructure.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="ImagesService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImagesServiceTests
{
    /// <summary>
    ///     The azure storage service mock.
    /// </summary>
    private readonly Mock<IAzureStorageService> _azureStorageServiceMock;

    /// <summary>
    ///     The images service.
    /// </summary>
    private readonly IImagesService _imagesService;

    /// <summary>
    ///     Setups ImagesServiceTests.
    /// </summary>
    public ImagesServiceTests()
    {
        _azureStorageServiceMock = new Mock<IAzureStorageService>();
        _imagesService = new Mock<ImagesService>(_azureStorageServiceMock.Object).Object;
    }

    /// <summary>
    ///     Tests that UploadImageAsync method returns image uri when image is valid.
    /// </summary>
    [Fact]
    public async Task UploadImageAsync_ShouldReturnImageUri_WhenImageIsValid()
    {
        // Arrange
        const string imagePath = "test/image.jpg";
        const string expectedUri = "https://example.com/image.jpg";
        var imageMock = new Mock<IFormFile>();

        _azureStorageServiceMock
            .Setup(x => x.UploadAsync(imagePath, imageMock.Object))
            .ReturnsAsync(new BlobResponseDto
            {
                Error = false,
                Status = "ok",
                Blob = new BlobDto { Uri = expectedUri, ContentType = "test" }
            });

        // Act
        var result = await _imagesService.UploadImageAsync(imagePath, imageMock.Object);

        // Assert
        result.Should().Be(expectedUri);
    }

    /// <summary>
    ///     Tests that UploadImageAsync method throws exception when upload error occurs.
    /// </summary>
    [Fact]
    public async Task UploadImageAsync_ShouldThrowException_WhenUploadErrorOccurs()
    {
        // Arrange
        const string imagePath = "test/test/test.jpg";
        var imageMock = new Mock<IFormFile>();

        _azureStorageServiceMock
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(new BlobResponseDto { Error = true });

        // Act
        var act = async () =>
            await _imagesService.UploadImageAsync(imagePath, imageMock.Object);

        // Assert
        await act.Should().ThrowAsync<RemoteServiceConnectionException>()
            .WithMessage("Failed to upload the image.");
    }

    /// <summary>
    ///     Tests that DeleteImageAsync method deletes image when image uri is valid.
    /// </summary>
    [Fact]
    public async Task DeleteImageAsync_ShouldDeleteImage_WhenImageUriIsValid()
    {
        // Arrange
        const string imageUri = "https://example.com/test/image.jpg";

        _azureStorageServiceMock
            .Setup(x => x.DeleteByUriAsync(imageUri))
            .ReturnsAsync(new BlobResponseDto { Error = false });

        // Act
        await _imagesService.DeleteImageAsync(imageUri);

        // Assert
        _azureStorageServiceMock.Verify(x => x.DeleteByUriAsync(It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    ///     Tests that DeleteImageAsync method throws exception when delete error occurs.
    /// </summary>
    [Fact]
    public async Task DeleteImageAsync_ShouldThrowException_WhenDeleteErrorOccurs()
    {
        // Arrange
        const string imageUri = "https://example.com/test/image.jpg";

        _azureStorageServiceMock
            .Setup(x => x.DeleteByUriAsync(imageUri))
            .ReturnsAsync(new BlobResponseDto { Error = true });

        // Act
        var act = async () => await _imagesService.DeleteImageAsync(imageUri);

        // Assert
        await act.Should().ThrowAsync<RemoteServiceConnectionException>()
            .WithMessage("Failed to delete the image.");
    }
}