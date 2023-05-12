﻿using Application.Beers.Commands.CreateBeer;
using Application.Beers.Dtos;
using Application.BeerStyles.Dtos;
using Application.Breweries.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Beers.Commands.CreateBeer;

/// <summary>
///     Unit tests for the <see cref="CreateBeerCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBeerCommandHandlerTests
{
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
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();
        _handler = new CreateBeerCommandHandler(_contextMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method creates beer and returns correct dto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateBeerAndReturnCorrectBeerDto()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var beerStyleId = Guid.NewGuid();
        var request = new CreateBeerCommand
        {
            Name = "Test beer",
            BreweryId = breweryId,
            AlcoholByVolume = 5.0,
            Description = "Test description",
            Blg = 12.0,
            Plato = 10.0,
            BeerStyleId = beerStyleId,
            Ibu = 25
        };
        var breweryDto = new BreweryDto { Id = breweryId };
        var beerStyleDto = new BreweryDto { Id = beerStyleId };
        var beers = Enumerable.Empty<Beer>();
        var beerDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var breweries = new List<Brewery> { new() { Id = breweryId } };
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        var beerStyles = new List<BeerStyle> { new() { Id = beerStyleId } };
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beerDbSetMock.Object);
        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BeerDto>();
        result.Name.Should().Be(request.Name);
        result.AlcoholByVolume.Should().Be(request.AlcoholByVolume);
        result.Description.Should().Be(request.Description);
        result.Blg.Should().Be(request.Blg);
        result.Plato.Should().Be(request.Plato);
        result.Ibu.Should().Be(request.Ibu);
        result.BeerStyle.Should().BeOfType<BeerStyleDto>();
        result.BeerStyle.Should().Be(beerStyleDto); //TODO verify, also add brewery
        
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when brewery does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBreweryDoesNotExists()
    {
        // Arrange
        var command = new CreateBeerCommand
        {
            Name = "Test Beer",
            BreweryId = Guid.NewGuid(),
            AlcoholByVolume = 5.0,
            Description = "A test beer",
            Blg = 12.0,
            Plato = 3.5,
            BeerStyleId = Guid.NewGuid(),
            Ibu = 30
        };
        var breweries = Enumerable.Empty<Brewery>();
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
    
    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer style does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerStyleDoesNotExists()
    {
        // Arrange
        var command = new CreateBeerCommand
        {
            Name = "Test Beer",
            BreweryId = Guid.NewGuid(),
            AlcoholByVolume = 5.0,
            Description = "A test beer",
            Blg = 12.0,
            Plato = 3.5,
            BeerStyleId = Guid.NewGuid(),
            Ibu = 30
        };
        var beerStyles = Enumerable.Empty<BeerStyle>();
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();
        //TODO verify that this work without breweries mock, it actually can work but exception will be thrown because of breweries, just add "WithMessage" here and above
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}