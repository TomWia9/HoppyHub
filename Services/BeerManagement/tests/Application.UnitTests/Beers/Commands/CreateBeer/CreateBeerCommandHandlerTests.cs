using Application.Beers.Commands.CreateBeer;
using Application.Beers.Dtos;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.Beers.Commands.CreateBeer;

/// <summary>
///     Unit tests for the <see cref="CreateBeerCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBeerCommandHandlerTests
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
    ///     The handler.
    /// </summary>
    private readonly CreateBeerCommandHandler _handler;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     Setups CreateBeerCommandHandlerTests.
    /// </summary>
    public CreateBeerCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _appConfigurationMock = new Mock<IAppConfiguration>();
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();
        _handler = new CreateBeerCommandHandler(_contextMock.Object, mapper, _publishEndpointMock.Object,
            _appConfigurationMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates beer, publishes BeerCreated event and returns correct dto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateBeerAndPublishBeerCreatedEventAndReturnCorrectBeerDto()
    {
        // Arrange
        const string tempImageUri = "test.com";
        const string beerName = "beer name";
        const string breweryName = "brewery name";
        var breweryId = Guid.NewGuid();
        var beerStyleId = Guid.NewGuid();
        var request = new CreateBeerCommand
        {
            Name = beerName,
            BreweryId = breweryId,
            AlcoholByVolume = 5.0,
            Description = "Test description",
            Composition = "Test composition",
            Blg = 12.0,
            BeerStyleId = beerStyleId,
            Ibu = 25,
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now)
        };
        var beers = Enumerable.Empty<Beer>();
        var beerDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var brewery = new Brewery { Id = breweryId, Name = breweryName };
        var beerStyles = new List<BeerStyle> { new() { Id = beerStyleId } };
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beerDbSetMock.Object);
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _appConfigurationMock.Setup(x => x.TempBeerImageUri).Returns(tempImageUri);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BeerDto>();
        result.Name.Should().Be(request.Name);
        result.AlcoholByVolume.Should().Be(request.AlcoholByVolume);
        result.Description.Should().Be(request.Description);
        result.Composition.Should().Be(request.Composition);
        result.Blg.Should().Be(request.Blg);
        result.Ibu.Should().Be(request.Ibu);
        result.ReleaseDate.Should().Be(request.ReleaseDate);
        result.ImageUri.Should().NotBeNull();

        _contextMock.Verify(x => x.Beers.AddAsync(It.IsAny<Beer>(), CancellationToken.None), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        _publishEndpointMock.Verify(x =>
            x.Publish(It.Is<BeerCreated>(y => y.Name == beerName && y.BreweryName == breweryName),
                It.IsAny<CancellationToken>()));
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when brewery does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBreweryDoesNotExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var command = new CreateBeerCommand
        {
            Name = "Test Beer",
            BreweryId = breweryId,
            AlcoholByVolume = 5.0,
            Description = "A test beer",
            Composition = "Test composition",
            Blg = 12.0,
            BeerStyleId = Guid.NewGuid(),
            Ibu = 30,
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now)
        };

        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brewery?)null);

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
        var command = new CreateBeerCommand
        {
            Name = "Test Beer",
            BreweryId = breweryId,
            AlcoholByVolume = 5.0,
            Description = "A test beer",
            Composition = "Test composition",
            Blg = 12.0,
            BeerStyleId = beerStyleId,
            Ibu = 30,
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now)
        };
        var brewery = new Brewery { Id = breweryId, Name = "Test name" };
        var beerStyles = Enumerable.Empty<BeerStyle>();
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(BeerStyle)}\" ({beerStyleId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}