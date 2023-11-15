using Application.Interfaces;
using Application.Services;
using Azure;
using Azure.Storage.Blobs;
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
        _blobStorageService = new BlobStorageService(loggerMock.Object, _blobContainerClientMock.Object);
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

    //TODO: Try to add more tests
}