using System.Linq.Expressions;
using Application.Beers.Dtos;
using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Favorites.Dtos;
using Application.Favorites.Queries.GetFavorites;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Favorites.Queries.GetFavorites;

/// <summary>
///     Unit tests for the <see cref="GetFavoritesQueryHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetFavoritesQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The QueryService mock.
    /// </summary>
    private readonly Mock<IQueryService<Favorite>> _queryServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetFavoritesQueryHandler _handler;

    /// <summary>
    ///     Setups GetFavoritesQueryHandlerTests.
    /// </summary>
    public GetFavoritesQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _queryServiceMock = new Mock<IQueryService<Favorite>>();
        _handler = new GetFavoritesQueryHandler(_contextMock.Object, _queryServiceMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method returns FavoritesListDto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFavoritesListDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var request = new GetFavoritesQuery { PageNumber = 1, PageSize = 10, UserId = userId };
        var beers = new List<Beer>
        {
            new()
            {
                Id = beerId,
                Name = "Test beer",
                AlcoholByVolume = 6,
                Blg = 15
            }
        };
        var favorites = new List<Favorite>
        {
            new()
            {
                Id = Guid.NewGuid(), BeerId = Guid.NewGuid(), Beer = beers[0], CreatedBy = userId,
            }
        };
        var expectedResult = new FavoritesListDto
        {
            UserId = userId,
            FavoriteBeers = beers.Select(x => new BeerDto
            {
                Id = x.Id,
                Name = x.Name,
                AlcoholByVolume = x.AlcoholByVolume,
                Blg = x.Blg
            }).ToPaginatedList(request.PageNumber, request.PageSize)
        };

        var favoritesDbSetMock = favorites.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Favorites).Returns(favoritesDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Filter(It.IsAny<IQueryable<Favorite>>(), It.IsAny<IEnumerable<Expression<Func<Favorite, bool>>>>()))
            .Returns(favoritesDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Sort(It.IsAny<IQueryable<Favorite>>(), It.IsAny<Expression<Func<Favorite, object>>>(),
                    It.IsAny<SortDirection>()))
            .Returns(favoritesDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<FavoritesListDto>();
        result.FavoriteBeers.Should().NotBeEmpty();
        result.FavoriteBeers!.Count.Should().Be(1);
        result.Should().BeEquivalentTo(expectedResult);
    }
}