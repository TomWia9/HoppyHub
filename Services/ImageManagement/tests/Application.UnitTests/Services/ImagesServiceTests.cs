using Application.Interfaces;
using Application.Models.BlobContainer;
using Application.Services;
using Moq;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="ImagesService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImagesServiceTests
{
    /// <summary>
    ///     The azure storage service mock.
    /// </summary>
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;

    /// <summary>
    ///     The images service.
    /// </summary>
    private readonly IImagesService _imagesService;

    /// <summary>
    ///     Setups ImagesServiceTests.
    /// </summary>
    public ImagesServiceTests()
    {
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _imagesService = new ImagesService(_blobStorageServiceMock.Object);
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
        var image = new byte[1];

        _blobStorageServiceMock
            .Setup(x => x.UploadAsync(imagePath, image))
            .ReturnsAsync(new BlobResponseDto
            {
                Error = false,
                Status = "ok",
                Blob = new BlobDto { Uri = expectedUri }
            });

        // Act
        var result = await _imagesService.UploadImageAsync(imagePath, image);

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
        var image = new byte[1];

        _blobStorageServiceMock
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
            .ReturnsAsync(new BlobResponseDto { Error = true });

        // Act
        var act = async () =>
            await _imagesService.UploadImageAsync(imagePath, image);

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

        _blobStorageServiceMock
            .Setup(x => x.DeleteByUriAsync(imageUri))
            .ReturnsAsync(new BlobResponseDto { Error = false });

        // Act
        await _imagesService.DeleteImageAsync(imageUri);

        // Assert
        _blobStorageServiceMock.Verify(x => x.DeleteByUriAsync(imageUri), Times.Once);
    }

    /// <summary>
    ///     Tests that DeleteImageAsync method throws exception when delete error occurs.
    /// </summary>
    [Fact]
    public async Task DeleteImageAsync_ShouldThrowException_WhenDeleteErrorOccurs()
    {
        // Arrange
        const string imageUri = "https://example.com/test/image.jpg";

        _blobStorageServiceMock
            .Setup(x => x.DeleteByUriAsync(imageUri))
            .ReturnsAsync(new BlobResponseDto { Error = true });

        // Act
        var act = async () => await _imagesService.DeleteImageAsync(imageUri);

        // Assert
        await act.Should().ThrowAsync<RemoteServiceConnectionException>()
            .WithMessage("Failed to delete the image.");
    }

    /// <summary>
    ///     Tests that DeleteAllImagesInPathAsync method calls DeleteInPathAsync method.
    /// </summary>
    [Fact]
    public async Task DeleteAllImagesInPathAsync_ShouldCallDeleteInPathAsync()
    {
        // Arrange
        const string path = "test/test";

        // Act
        await _imagesService.DeleteAllImagesInPathAsync(path);

        // Assert
        _blobStorageServiceMock.Verify(x => x.DeleteInPathAsync(path), Times.Once);
    }
}