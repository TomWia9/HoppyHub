﻿using Application.Beers.Commands.CreateBeer;
using Application.Beers.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Mappings;

namespace Application.UnitTests.Beers.Commands.CreateBeer;

/// <summary>
///     Unit tests for the <see cref="CreateBeerCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBeerCommandHandlerTests
{
    /// <summary>
    ///     The beers images service mock.
    /// </summary>
    private readonly Mock<IBeersImagesService> _beersImagesServiceMock;

    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly CreateBeerCommandHandler _handler;

    /// <summary>
    ///     Setups CreateBeerCommandHandlerTests.
    /// </summary>
    public CreateBeerCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _beersImagesServiceMock = new Mock<IBeersImagesService>();
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();
        _handler = new CreateBeerCommandHandler(_contextMock.Object, mapper, _beersImagesServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates beer and returns correct dto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateBeerAndReturnCorrectBeerDto()
    {
        // Arrange
        const string tempImageUri = "test.com";
        var breweryId = Guid.NewGuid();
        var beerStyleId = Guid.NewGuid();
        var request = new CreateBeerCommand
        {
            Name = "Test beer",
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
        var breweries = new List<Brewery> { new() { Id = breweryId } };
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        var beerStyles = new List<BeerStyle> { new() { Id = beerStyleId } };
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beerDbSetMock.Object);
        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _beersImagesServiceMock.Setup(x => x.GetTempBeerImageUri()).Returns(tempImageUri);

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