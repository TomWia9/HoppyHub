using System.Linq.Expressions;
using Application.Beers.Dtos;
using Application.Beers.Queries.GetBeers;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Enums;
using SharedUtilities.Interfaces;
using SharedUtilities.Models;

namespace Application.UnitTests.Beers.Queries.GetBeers;

/// <summary>
///     Unit tests for the <see cref="GetBeersQueryHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBeersQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetBeersQueryHandler _handler;

    /// <summary>
    ///     The QueryService mock.
    /// </summary>
    private readonly Mock<IQueryService<Beer>> _queryServiceMock;

    /// <summary>
    ///     Setups GetBeersQueryHandlerTests.
    /// </summary>
    public GetBeersQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        Mock<IFilteringHelper<Beer, GetBeersQuery>> filteringHelperMock = new();
        _contextMock = new Mock<IApplicationDbContext>();
        _queryServiceMock = new Mock<IQueryService<Beer>>();
        _handler = new GetBeersQueryHandler(_contextMock.Object, _queryServiceMock.Object, mapper,
            filteringHelperMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method returns PaginatedList of BeerDto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnPaginatedListOfBeerDto()
    {
        // Arrange
        const string imageUri = "https://test.com/test.jpg";
        var request = new GetBeersQuery { PageNumber = 1, PageSize = 10 };
        var beerImage = new BeerImage { ImageUri = imageUri };

        var beers = new List<Beer>
        {
            new() { Id = Guid.NewGuid(), Name = "Beer 1", AlcoholByVolume = 5, BeerImage = beerImage },
            new() { Id = Guid.NewGuid(), Name = "Beer 2", AlcoholByVolume = 6, BeerImage = beerImage },
            new() { Id = Guid.NewGuid(), Name = "Beer 3", AlcoholByVolume = 7, BeerImage = beerImage }
        };

        var expectedResult = PaginatedList<BeerDto>.Create(beers.Select(x => new BeerDto
        {
            Id = x.Id,
            Name = x.Name,
            AlcoholByVolume = x.AlcoholByVolume,
            ImageUri = x.BeerImage?.ImageUri
        }), 1, 10);

        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Filter(It.IsAny<IQueryable<Beer>>(), It.IsAny<IEnumerable<Expression<Func<Beer, bool>>>>()))
            .Returns(beersDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Sort(It.IsAny<IQueryable<Beer>>(), It.IsAny<Expression<Func<Beer, object>>>(),
                    It.IsAny<SortDirection>()))
            .Returns(beersDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PaginatedList<BeerDto>>();
        result.Count.Should().Be(3);
        result.Should().BeEquivalentTo(expectedResult);
    }

    /// <summary>
    ///     Tests that Handle method returns correct PaginatedList of BeerDto when MinReleaseDate and MaxReleaseDate specified.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnCorrectPaginatedListOfBeerDtoWhenMinAndMaxReleaseDateSpecified()
    {
        // Arrange
        const string imageUri = "https://test.com/test.jpg";
        var request = new GetBeersQuery
        {
            MinReleaseDate = DateOnly.Parse("01.01.2010"),
            MaxReleaseDate = DateOnly.Parse("01.01.2012")
        };
        var beerImage = new BeerImage { ImageUri = imageUri };
        var allBeers = new List<Beer>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "Beer 1", ReleaseDate = DateOnly.Parse("01.01.2009"), BeerImage = beerImage
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Beer 2", ReleaseDate = DateOnly.Parse("01.02.2010"), BeerImage = beerImage
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Beer 3", ReleaseDate = DateOnly.Parse("01.04.2011"), BeerImage = beerImage
            }
        };
        var expectedBeers = new List<Beer>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "Beer 2", ReleaseDate = DateOnly.Parse("01.02.2010"), BeerImage = beerImage
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Beer 3", ReleaseDate = DateOnly.Parse("01.04.2011"), BeerImage = beerImage
            }
        };

        var expectedResult = PaginatedList<BeerDto>.Create(expectedBeers.Select(x =>
            new BeerDto
            {
                Id = x.Id,
                Name = x.Name,
                ReleaseDate = x.ReleaseDate,
                ImageUri = x.BeerImage?.ImageUri
            }), 1, 10);

        var allBeersDbSetMock = allBeers.AsQueryable().BuildMockDbSet();
        var expectedBeersDbSetMock = expectedBeers.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(allBeersDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Filter(It.IsAny<IQueryable<Beer>>(), It.IsAny<IEnumerable<Expression<Func<Beer, bool>>>>()))
            .Returns(expectedBeersDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Sort(It.IsAny<IQueryable<Beer>>(), It.IsAny<Expression<Func<Beer, object>>>(),
                    It.IsAny<SortDirection>()))
            .Returns(expectedBeersDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PaginatedList<BeerDto>>();
        result.Count.Should().Be(2, "beer with incorrect release date should not be returned");
        foreach (var beer in result)
        {
            beer.ReleaseDate.Should().BeOnOrAfter(request.MinReleaseDate);
            beer.ReleaseDate.Should().BeOnOrBefore(request.MaxReleaseDate);
        }

        result.Should().BeEquivalentTo(expectedResult,
            "beers release dates should be in requested min and max release dates");
    }
}