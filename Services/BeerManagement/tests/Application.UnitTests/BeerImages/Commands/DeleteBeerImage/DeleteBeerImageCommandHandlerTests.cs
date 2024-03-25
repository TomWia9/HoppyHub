using Application.BeerImages.Commands.DeleteBeerImage;
using Application.Common.Interfaces;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     Unit tests for the <see cref="DeleteBeerImageCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBeerImageCommandHandlerTests
{
    /// <summary>
    ///     The app configuration mock.
    /// </summary>
    private readonly Mock<IAppConfiguration> _appConfigurationMock;

    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The storage container service mock.
    /// </summary>
    private readonly Mock<IStorageContainerService> _storageContainerServiceMock;

    /// <summary>
    ///     The DeleteBeerImageCommand handler.
    /// </summary>
    private readonly DeleteBeerImageCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteBeerImageCommandHandlerTests.
    /// </summary>
    public DeleteBeerImageCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _storageContainerServiceMock = new Mock<IStorageContainerService>();
        _appConfigurationMock = new Mock<IAppConfiguration>();

        _handler = new DeleteBeerImageCommandHandler(_contextMock.Object, _storageContainerServiceMock.Object,
            _appConfigurationMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method deletes beer image and change beer image to temp when beer and beer image exists.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldDeleteBeerImageAndChangeBeerImageToTemp_WhenBeerAndBeerImageExists()
    {
        // Arrange
        const string tempImageUri = "https://test.com/temp.jpg";
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "test.com", TempImage = false };
        var beer = new Beer { Id = beerId, BreweryId = breweryId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand { BeerId = beerId };

        _appConfigurationMock.SetupGet(x => x.TempBeerImageUri).Returns(tempImageUri);
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _storageContainerServiceMock.Verify(x => x.DeleteFromPathAsync(It.IsAny<string>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beers = Enumerable.Empty<Beer>();
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand
        {
            BeerId = beerId
        };

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that Handle method throws BadRequestException when beer image is already deleted.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenBeerImageIsAlreadyDeleted()
    {
        // Arrange
        const string expectedMessage = "Image already deleted.";
        var beerId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "temp.jpg", TempImage = true };
        var beer = new Beer { Id = beerId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand
        {
            BeerId = beerId
        };

        _contextMock
            .Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<BadRequestException>().WithMessage(expectedMessage);
    }
}