using Application.Beers.Commands.UpdateBeer;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Beers.Commands.UpdateBeer;

/// <summary>
///     Unit tests for the <see cref="UpdateBeerCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateBeerCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

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
        _handler = new UpdateBeerCommandHandler(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method updates beer when beer exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateBeer_WhenBeerExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var beerStyleId = Guid.NewGuid();
        var breweries = new List<Brewery> { new() { Id = breweryId } };
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        var beerStyles = new List<BeerStyle> { new() { Id = beerStyleId } };
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();
        var beerId = Guid.NewGuid();
        var existingBeer = new Beer { Id = beerId, Name = "Old Name" };

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBeer);

        var command = new UpdateBeerCommand
            { Id = beerId, Name = "New Name", BreweryId = breweryId, BeerStyleId = beerStyleId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
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

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { 1 }, CancellationToken.None))
            .ReturnsAsync((Beer?)null);

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