using System.Linq.Expressions;
using Application.Breweries.Dtos;
using Application.Breweries.Queries.GetBreweries;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Enums;
using SharedUtilities.Models;

namespace Application.UnitTests.Breweries.Queries.GetBreweries;

/// <summary>
///     Unit tests for the <see cref="GetBreweriesQueryHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBreweriesQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetBreweriesQueryHandler _handler;

    /// <summary>
    ///     The QueryService mock.
    /// </summary>
    private readonly Mock<IQueryService<Brewery>> _queryServiceMock;

    /// <summary>
    ///     Setups GetBreweriesQueryHandlerTests.
    /// </summary>
    public GetBreweriesQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        Mock<IFilteringHelper<Brewery, GetBreweriesQuery>> filteringHelperMock = new();
        _contextMock = new Mock<IApplicationDbContext>();
        _queryServiceMock = new Mock<IQueryService<Brewery>>();
        _handler = new GetBreweriesQueryHandler(_contextMock.Object, _queryServiceMock.Object, mapper,
            filteringHelperMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method returns PaginatedList of BreweryDto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnPaginatedListOfBreweryDto()
    {
        // Arrange
        var request = new GetBreweriesQuery { PageNumber = 1, PageSize = 10 };

        var breweries = new List<Brewery>
        {
            new() { Id = Guid.NewGuid(), Name = "Brewery 1", FoundationYear = 1990 },
            new() { Id = Guid.NewGuid(), Name = "Brewery 2", FoundationYear = 2010 },
            new() { Id = Guid.NewGuid(), Name = "Brewery 3", FoundationYear = 2015 }
        };

        var expectedResult = PaginatedList<BreweryDto>.Create(breweries.Select(x => new BreweryDto
        {
            Id = x.Id,
            Name = x.Name,
            FoundationYear = x.FoundationYear
        }), 1, 10);

        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Filter(It.IsAny<IQueryable<Brewery>>(), It.IsAny<IEnumerable<Expression<Func<Brewery, bool>>>>()))
            .Returns(breweriesDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Sort(It.IsAny<IQueryable<Brewery>>(), It.IsAny<Expression<Func<Brewery, object>>>(),
                    It.IsAny<SortDirection>()))
            .Returns(breweriesDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PaginatedList<BreweryDto>>();
        result.Count.Should().Be(3);
        result.Should().BeEquivalentTo(expectedResult);
    }
}