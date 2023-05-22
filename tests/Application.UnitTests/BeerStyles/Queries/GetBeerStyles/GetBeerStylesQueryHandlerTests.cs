using System.Linq.Expressions;
using Application.BeerStyles.Dtos;
using Application.BeerStyles.Queries.GetBeerStyles;
using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.BeerStyles.Queries.GetBeerStyles;

/// <summary>
///     Unit tests for the <see cref="GetBeerStylesQueryHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBeerStylesQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The QueryService mock.
    /// </summary>
    private readonly Mock<IQueryService<BeerStyle>> _queryServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetBeerStylesQueryHandler _handler;

    /// <summary>
    ///     Setups GetBeerStylesQueryHandlerTests.
    /// </summary>
    public GetBeerStylesQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });

        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _queryServiceMock = new Mock<IQueryService<BeerStyle>>();
        _handler = new GetBeerStylesQueryHandler(_contextMock.Object, _queryServiceMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method returns PaginatedList of BeerStyleDto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnPaginatedListOfBeerStyleDto()
    {
        // Arrange
        var request = new GetBeerStylesQuery { PageNumber = 1, PageSize = 10 };

        var beersStyles = new List<BeerStyle>
        {
            new() { Id = Guid.NewGuid(), Name = "IPA", Description = "test description", CountryOfOrigin = "England" },
            new() { Id = Guid.NewGuid(), Name = "APA", Description = "test description", CountryOfOrigin = "USA" },
            new() { Id = Guid.NewGuid(), Name = "PILS", Description = "test description", CountryOfOrigin = "Germany" }
        };

        var expectedResult = PaginatedList<BeerStyleDto>.Create(beersStyles.Select(x => new BeerStyleDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            CountryOfOrigin = x.CountryOfOrigin
        }), 1, 10);

        var beerStylesDbSetMock = beersStyles.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Filter(It.IsAny<IQueryable<BeerStyle>>(), It.IsAny<IEnumerable<Expression<Func<BeerStyle, bool>>>>()))
            .Returns(beerStylesDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Sort(It.IsAny<IQueryable<BeerStyle>>(), It.IsAny<Expression<Func<BeerStyle, object>>>(),
                    It.IsAny<SortDirection>()))
            .Returns(beerStylesDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PaginatedList<BeerStyleDto>>();
        result.Count.Should().Be(3);
        result.Should().BeEquivalentTo(expectedResult);
    }
}