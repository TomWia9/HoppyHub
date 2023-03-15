﻿using System.Linq.Expressions;
using Application.Beers;
using Application.Beers.Queries.GetBeers;
using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Beers.Queries.GetBeers;

/// <summary>
///     Unit tests for the <see cref="GetBeersQueryHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBeersQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The QueryService mock.
    /// </summary>
    private readonly Mock<IQueryService<Beer>> _queryServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetBeersQueryHandler _handler;

    /// <summary>
    ///     Setups GetBeersQueryHandlerTests.
    /// </summary>
    public GetBeersQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });

        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _queryServiceMock = new Mock<IQueryService<Beer>>();
        _handler = new GetBeersQueryHandler(_contextMock.Object, _queryServiceMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method returns PaginatedList of BeerDto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnPaginatedListBeerDto()
    {
        // Arrange
        var request = new GetBeersQuery { PageNumber = 1, PageSize = 10 };

        var beers = new List<Beer>
        {
            new() { Id = Guid.NewGuid(), Name = "Beer 1", AlcoholByVolume = 5 },
            new() { Id = Guid.NewGuid(), Name = "Beer 2", AlcoholByVolume = 6 },
            new() { Id = Guid.NewGuid(), Name = "Beer 3", AlcoholByVolume = 7 }
        };

        var expectedResult = PaginatedList<BeerDto>.Create(beers.Select(x => new BeerDto
        {
            Id = x.Id,
            Name = x.Name,
            AlcoholByVolume = x.AlcoholByVolume
        }), 1, 10);

        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var beersQueryableMock = beers.AsQueryable().BuildMock();

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _queryServiceMock.Setup(qs =>
                qs.Filter(It.IsAny<IQueryable<Beer>>(), It.IsAny<IEnumerable<Expression<Func<Beer, bool>>>>()))
            .Returns(beersQueryableMock);
        _queryServiceMock.Setup(qs =>
                qs.Sort(It.IsAny<IQueryable<Beer>>(), It.IsAny<Expression<Func<Beer, object>>>(),
                    It.IsAny<SortDirection>()))
            .Returns(beersQueryableMock);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PaginatedList<BeerDto>>();
        result.Count.Should().Be(3);
        result.Should().BeEquivalentTo(expectedResult);
    }
}