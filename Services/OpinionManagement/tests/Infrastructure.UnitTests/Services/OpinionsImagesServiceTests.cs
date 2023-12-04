// using Application.Common.Interfaces;
// using Infrastructure.Services;
// using Microsoft.AspNetCore.Http;
// using Moq;
//
// namespace Infrastructure.UnitTests.Services;
//
// /// <summary>
// ///     Tests for the <see cref="OpinionsImagesService" /> class.
// /// </summary>
// [ExcludeFromCodeCoverage]
// public class OpinionsImagesServiceTests
// {
//     /// <summary>
//     ///     The opinions images service.
//     /// </summary>
//     private readonly IOpinionsImagesService _opinionsImagesService;
//
//     /// <summary>
//     ///     Setups OpinionsImagesServiceTests.
//     /// </summary>
//     public OpinionsImagesServiceTests()
//     {
//         Mock<IAzureStorageService> azureStorageServiceMock = new();
//         _opinionsImagesService = new OpinionsImagesService(azureStorageServiceMock.Object);
//     }
//
//     /// <summary>
//     ///     Tests that CreateImagePath method returns beer image path.
//     /// </summary>
//     [Fact]
//     public void CreateImagePath_ShouldReturnBeerImagePath()
//     {
//         // Arrange
//         const string extension = ".jpg";
//
//         var breweryId = Guid.NewGuid();
//         var beerId = Guid.NewGuid();
//         var opinionId = Guid.NewGuid();
//         var expectedPath = $"Opinions/{breweryId.ToString()}/{beerId.ToString()}/{opinionId.ToString()}" + extension;
//
//         var imageMock = new Mock<IFormFile>();
//         imageMock.SetupGet(x => x.FileName).Returns("test.jpg");
//
//         // Act
//         var result = _opinionsImagesService.CreateImagePath(imageMock.Object, breweryId, beerId, opinionId);
//
//         // Assert
//         result.Should().Be(expectedPath);
//     }
// }

