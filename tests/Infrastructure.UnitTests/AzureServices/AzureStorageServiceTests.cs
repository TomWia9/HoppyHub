using Application.Common.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Infrastructure.AzureServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.UnitTests.AzureServices;

/// <summary>
///     Tests for the <see cref="AzureStorageService"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class AzureStorageServiceTests
{
    /// <summary>
    ///     The blob container client mock.
    /// </summary>
    private readonly Mock<BlobContainerClient> _blobContainerClientMock;

    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     Setups AzureStorageServiceTests.
    /// </summary>
    public AzureStorageServiceTests()
    {
        Mock<ILogger<AzureStorageService>> loggerMock = new();
        _blobContainerClientMock = new Mock<BlobContainerClient>();

        _azureStorageService = new AzureStorageService(loggerMock.Object, _blobContainerClientMock.Object);
    }

    /// <summary>
    ///     Tests that UploadAsync method returns blob response dto with error status when upload fails.
    /// </summary>
    [Fact]
    public async Task UploadAsync_ShouldReturnBlobResponseDtoWithErrorStatus_WhenUploadFails()
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();

        _blobContainerClientMock.Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Throws(new RequestFailedException("Upload failed"));

        // Act
        var response = await _azureStorageService.UploadAsync(formFileMock.Object);

        // Assert
        response.Error.Should().BeTrue();
    }
}