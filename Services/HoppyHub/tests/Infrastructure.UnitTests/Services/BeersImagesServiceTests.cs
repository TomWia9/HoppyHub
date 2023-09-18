using Application.Common.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Infrastructure.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="BeersImagesService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeersImagesServiceTests
{
    /// <summary>
    ///     The beers images service.
    /// </summary>
    private readonly IBeersImagesService _beersImagesService;

    /// <summary>
    ///     Setups BeersImagesServiceTests.
    /// </summary>
    public BeersImagesServiceTests()
    {
        var inMemoryConfiguration = new Dictionary<string, string>
        {
            { "TempBeerImageUri", "https://test.com/tempImage.jpg" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfiguration!)
            .Build();

        Mock<IAzureStorageService> azureStorageServiceMock = new();
        _beersImagesService = new BeersImagesService(azureStorageServiceMock.Object, configuration);
    }

    /// <summary>
    ///     Tests that GetBeerImageTempUri method returns image uri when image is valid.
    /// </summary>
    [Fact]
    public void GetTempBeerImageUri_ShouldReturnTempImageUri()
    {
        // Arrange
        const string expectedUri = "https://test.com/tempImage.jpg";

        // Act
        var result = _beersImagesService.GetTempBeerImageUri();

        // Assert
        result.Should().Be(expectedUri);
    }

    /// <summary>
    ///     Tests that CreateImagePath method returns beer image path.
    /// </summary>
    [Fact]
    public void CreateImagePath_ShouldReturnBeerImagePath()
    {
        // Arrange
        const string extension = ".jpg";

        var breweryId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var expectedPath = $"Beers/{breweryId.ToString()}/{beerId.ToString()}" + extension;

        var imageMock = new Mock<IFormFile>();
        imageMock.SetupGet(x => x.FileName).Returns("test.jpg");

        // Act
        var result = _beersImagesService.CreateImagePath(imageMock.Object, breweryId, beerId);

        // Assert
        result.Should().Be(expectedPath);
    }
}