using Application.BeerImages.Commands.DeleteBeerImage;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     Unit tests for the <see cref="DeleteBeerImageCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBeerImageCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

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
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _handler = new DeleteBeerImageCommandHandler(_contextMock.Object, _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method publishes ImagesDeleted event when beer and
    ///     beer image exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldPublishImagesDeletedEvent_WhenBeerAndBeerImageExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beerImage = new BeerImage { BeerId = beerId, ImageUri = "test.com", TempImage = false };
        var beer = new Beer { Id = beerId, BeerImage = beerImage };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var command = new DeleteBeerImageCommand { BeerId = beerId };
        var expectedEvent = new ImagesDeleted
        {
            Paths = new List<string>
            {
                $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}"
            }
        };

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _publishEndpointMock.Verify(x => x.Publish(It.Is<ImagesDeleted>(y =>
            y.Paths!.Count() == expectedEvent.Paths.Count() &&
            y.Paths!.All(expectedEvent.Paths.Contains)), It.IsAny<CancellationToken>()), Times.Once);
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

        _contextMock
            .Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}