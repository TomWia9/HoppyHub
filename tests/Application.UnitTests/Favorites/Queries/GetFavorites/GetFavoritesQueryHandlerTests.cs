using System.Linq.Expressions;
using Application.Beers.Dtos;
using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
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

        Mock<IFilteringHelper<Favorite, GetFavoritesQuery>> filteringHelperMock = new();
        _contextMock = new Mock<IApplicationDbContext>();
        _queryServiceMock = new Mock<IQueryService<Favorite>>();
        _handler = new GetFavoritesQueryHandler(_contextMock.Object, _queryServiceMock.Object, mapper,
            filteringHelperMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method returns favorite beers of specified user.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFavoritesListDto()
    {
        // Arrange
        const string imageUri = "https://test.com/test.jpg";
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
                Blg = 15,
                BeerImage = new BeerImage { ImageUri = imageUri }
            }
        };
        var favorites = new List<Favorite>
        {
            new()
            {
                Id = Guid.NewGuid(), BeerId = Guid.NewGuid(), Beer = beers[0], CreatedBy = userId,
            }
        };
        var favoritesDbSetMock = favorites.AsQueryable().BuildMockDbSet();
        var expectedResult = PaginatedList<BeerDto>.Create(beers.Select(x => new BeerDto
        {
            Id = x.Id,
            Name = x.Name,
            AlcoholByVolume = x.AlcoholByVolume,
            Blg = x.Blg,
            ImageUri = x.BeerImage?.ImageUri
        }), 1, 10);

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
        result.Should().BeOfType<PaginatedList<BeerDto>>();
        result.Should().NotBeEmpty();
        result.Count.Should().Be(1);
        result.Should().BeEquivalentTo(expectedResult);
    }
}