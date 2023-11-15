using Application.Interfaces;
using Application.Services;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="BlobStorageService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BlobStorageServiceTests
{
    /// <summary>
    ///     The blob container client mock.
    /// </summary>
    private readonly Mock<BlobContainerClient> _blobContainerClientMock;

    /// <summary>
    /// The blob client mock.
    /// </summary>
    private readonly Mock<BlobClient> _blobClientMock;

    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IBlobStorageService _blobStorageService;

    /// <summary>
    ///     Setups BlobStorageServiceTests.
    /// </summary>
    public BlobStorageServiceTests()
    {
        Mock<ILogger<BlobStorageService>> loggerMock = new();
        _blobContainerClientMock = new Mock<BlobContainerClient>();
        _blobClientMock = new Mock<BlobClient>();
        _blobStorageService = new BlobStorageService(loggerMock.Object, _blobContainerClientMock.Object);
    }

    /// <summary>
    ///     Tests that UploadAsync method returns BlobResponseDto without error when upload succeed.
    /// </summary>
    [Fact]
    public async Task UploadAsync_ShouldReturnBlobResponseDtoWithWithoutError_WhenUploadSucceed()
    {
        // Arrange
        const string path = "test/test";
        const string name = "blobName";
        var uri = new Uri("https://test.com/test.jpg");
        var blobData = new byte[1];

        _blobClientMock.SetupGet(x => x.Uri).Returns(uri);
        _blobClientMock.SetupGet(x => x.Name).Returns(name);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(path))
            .Returns(_blobClientMock.Object);

        // Act
        var response = await _blobStorageService.UploadAsync(path, blobData);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be("File uploaded successfully");
        response.Error.Should().BeFalse();
        response.Blob.Should().NotBeNull();
        response.Blob.Uri.Should().Be(uri.ToString());
        response.Blob.Name.Should().Be(name);
    }

    /// <summary>
    ///     Tests that UploadAsync method returns blob response dto with error status when upload fails.
    /// </summary>
    [Fact]
    public async Task UploadAsync_ShouldReturnBlobResponseDtoWithErrorStatus_WhenUploadFails()
    {
        // Arrange
        const string path = "test/test";
        var file = new byte[1];

        _blobContainerClientMock.Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Throws(new RequestFailedException("Upload failed"));

        // Act
        var response = await _blobStorageService.UploadAsync(path, file);

        // Assert
        response.Error.Should().BeTrue();
    }

    /// <summary>
    ///     Tests that DeleteAsync method returns BlobResponseDto without error when blob exists.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnBlobResponseDtoWithoutError_WhenBlobExists()
    {
        // Arrange
        const string path = "test/test";
        var mockResponse = new Mock<Response<bool>>();

        mockResponse.SetupGet(x => x.Value).Returns(true);
        _blobClientMock.Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(),
                It.IsAny<BlobRequestConditions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        _blobContainerClientMock.Setup(x => x.GetBlobClient(path))
            .Returns(_blobClientMock.Object);

        // Act
        var response = await _blobStorageService.DeleteAsync(path);

        // Assert
        response.Should().NotBeNull();
        response.Error.Should().BeFalse();
        response.Status.Should().Be($"File: {path} has been successfully deleted.");
    }

    /// <summary>
    ///     Tests that DeleteAsync returns BlobResponseDto with error and correct status when blob does not exists.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnBlobResponseDtoWithErrorAndCorrectStatus_WhenBlobDoesNotExists()
    {
        // Arrange
        const string path = "test/test";
        var mockResponse = new Mock<Response<bool>>();

        mockResponse.SetupGet(x => x.Value).Returns(false);
        _blobClientMock.Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(),
                It.IsAny<BlobRequestConditions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(path))
            .Returns(_blobClientMock.Object);

        // Act
        var response = await _blobStorageService.DeleteAsync(path);

        // Assert
        response.Should().NotBeNull();
        response.Error.Should().BeTrue();
        response.Status.Should().Contain($"File with name {path} not found.");
    }

    /// <summary>
    ///     Tests that DeleteAsync returns BlobResponseDto with error and correct status when RequestFailed exception was thrown.
    /// </summary>
    [Fact]
    public async Task
        DeleteAsync_ShouldReturnBlobResponseDtoWithErrorAndCorrectStatus_WhenRequestFailedExceptionWasThrown()
    {
        // Arrange
        const string path = "test/test";
        _blobClientMock.Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(),
                It.IsAny<BlobRequestConditions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(400, "Bad request"));
        _blobContainerClientMock.Setup(x => x.GetBlobClient(path))
            .Returns(_blobClientMock.Object);

        // Act
        var response = await _blobStorageService.DeleteAsync(path);

        // Assert
        response.Should().NotBeNull();
        response.Error.Should().BeTrue();
        response.Status.Should().Contain($"A problem occurred while deleting a {path} file.");
    }

    /// <summary>
    ///     Tests that DeleteByUriAsync method returns BlobResponseDto without error when blob exists.
    /// </summary>
    [Fact]
    public async Task DeleteByUriAsync_ShouldReturnBlobResponseDtoWithoutError_WhenBlobExists()
    {
        // Arrange
        const string uri = "https://test.com/test.jpg";
        var mockResponse = new Mock<Response<bool>>();

        mockResponse.SetupGet(x => x.Value).Returns(true);
        _blobClientMock.Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(),
                It.IsAny<BlobRequestConditions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        // Act
        var response = await _blobStorageService.DeleteByUriAsync(uri);

        // Assert
        response.Should().NotBeNull();
        response.Error.Should().BeFalse();
    }
}