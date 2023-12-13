using Application.Beers.Commands.UpdateBeer;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.Beers.Commands.UpdateBeer;

/// <summary>
///     Unit tests for the <see cref="UpdateBeerCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateBeerCommandHandlerTests
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
    ///     The handler.
    /// </summary>
    private readonly UpdateBeerCommandHandler _handler;

    /// <summary>
    ///     Setups UpdateBeerCommandHandlerTests.
    /// </summary>
    public UpdateBeerCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new UpdateBeerCommandHandler(_contextMock.Object, _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method updates beer and publishes BeerUpdated event when beer exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateBeerAndPublishBeerUpdatedEvent_WhenBeerExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var beerStyleId = Guid.NewGuid();
        var brewery = new Brewery
        {
            Id = breweryId,
            Name = "Brewery name"
        };
        var beerStyle = new BeerStyle
        {
            Id = beerStyleId,
            Name = "Beer style name"
        };
        var breweries = new List<Brewery> { brewery };
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        var beerStyles = new List<BeerStyle> { beerStyle };
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();
        var beerId = Guid.NewGuid();
        var existingBeer = new Beer { Id = beerId, Name = "Old Name", Brewery = brewery, BeerStyle = beerStyle };
        var beers = new List<Beer> { existingBeer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        var command = new UpdateBeerCommand
        {
            Id = beerId,
            Name = "New Name",
            BreweryId = breweryId,
            BeerStyleId = beerStyleId,
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now)
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingBeer.Name.Should().Be(command.Name);
        existingBeer.BreweryId.Should().Be(command.BreweryId);
        existingBeer.BeerStyleId.Should().Be(command.BeerStyleId);
        existingBeer.ReleaseDate.Should().Be(command.ReleaseDate);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        _publishEndpointMock.Verify(x =>
            x.Publish(It.Is<BeerUpdated>(y => y.Name == command.Name), It.IsAny<CancellationToken>()));
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExist()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beerStyleId = Guid.NewGuid();
        var breweries = new List<Brewery> { new() { Id = breweryId } };
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        var beerStyles = new List<BeerStyle> { new() { Id = beerStyleId } };
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();
        var beers = Enumerable.Empty<Beer>();
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        var command = new UpdateBeerCommand
            { Id = beerId, Name = "New Name", BreweryId = breweryId, BeerStyleId = beerStyleId };

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when brewery does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBreweryDoesNotExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var command = new UpdateBeerCommand
        {
            Name = "Test Beer",
            BreweryId = breweryId
        };
        var breweries = Enumerable.Empty<Brewery>();
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(Brewery)}\" ({breweryId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer style does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerStyleDoesNotExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var beerStyleId = Guid.NewGuid();
        var command = new UpdateBeerCommand
        {
            Name = "Test Beer",
            BreweryId = breweryId,
            BeerStyleId = beerStyleId
        };
        var breweries = new List<Brewery> { new() { Id = breweryId } };
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        var beerStyles = Enumerable.Empty<BeerStyle>();
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(BeerStyle)}\" ({beerStyleId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}