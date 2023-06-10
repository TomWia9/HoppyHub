using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models.BlobContainer;
using Application.Opinions.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.UnitTests.Opinions.Services;

/// <summary>
///     Unit tests for the <see cref="OpinionsService"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class OpinionsServiceTests
{
    /// <summary>
    ///     The azure storage service mock.
    /// </summary>
    private readonly Mock<IAzureStorageService> _azureStorageServiceMock;

    /// <summary>
    ///     The opinions service.
    /// </summary>
    private readonly IOpinionsService _opinionsService;

    /// <summary>
    ///     Setups OpinionsServiceTests.
    /// </summary>
    public OpinionsServiceTests()
    {
        Mock<ICurrentUserService> currentUserServiceMock = new();
        currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _azureStorageServiceMock = new Mock<IAzureStorageService>();

        _opinionsService = new OpinionsService(currentUserServiceMock.Object, _azureStorageServiceMock.Object);
    }

    /// <summary>
    ///     Tests that UploadOpinionImageAsync method returns image uri when image is valid.
    /// </summary>
    [Fact]
    public async Task UploadOpinionImageAsync_ShouldReturnImageUri_WhenImageIsValid()
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
        var result = await _opinionsService.UploadOpinionImageAsync(imageMock.Object, breweryId, beerId);

        // Assert
        result.Should().Be(expectedUri);
    }

    /// <summary>
    ///     Tests that UploadOpinionImageAsync method throws exception when upload error occurs.
    /// </summary>
    [Fact]
    public async Task UploadOpinionImageAsync_ShouldThrowException_WhenUploadErrorOccurs()
    {
        // Arrange
        var imageMock = new Mock<IFormFile>();
        var breweryId = Guid.NewGuid();
        var beerId = Guid.NewGuid();

        _azureStorageServiceMock
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(new BlobResponseDto { Error = true });

        // Act
        var act = async () =>
            await _opinionsService.UploadOpinionImageAsync(imageMock.Object, breweryId, beerId);

        // Assert
        await act.Should().ThrowAsync<RemoteServiceConnectionException>()
            .WithMessage("Failed to upload the image. The opinion was not saved.");
    }

    /// <summary>
    ///     Tests that DeleteOpinionImageAsync method deletes image when image uri is valid.
    /// </summary>
    [Fact]
    public async Task DeleteOpinionImageAsync_ShouldDeleteImage_WhenImageUriIsValid()
    {
        // Arrange
        const string imageUri = "https://example.com/Opinions/image.jpg";

        _azureStorageServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(new BlobResponseDto { Error = false });

        // Act
        await _opinionsService.DeleteOpinionImageAsync(imageUri);

        // Assert
        _azureStorageServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    ///     Tests that DeleteOpinionImageAsync method throws exception when delete error occurs.
    /// </summary>
    [Fact]
    public async Task DeleteOpinionImageAsync_ShouldThrowException_WhenDeleteErrorOccurs()
    {
        // Arrange
        const string imageUri = "https://example.com/Opinions/image.jpg";

        _azureStorageServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(new BlobResponseDto { Error = true });

        // Act
        var act = async () => await _opinionsService.DeleteOpinionImageAsync(imageUri);

        // Assert
        await act.Should().ThrowAsync<RemoteServiceConnectionException>()
            .WithMessage("Failed to delete the image. The opinion was not deleted.");
    }
}