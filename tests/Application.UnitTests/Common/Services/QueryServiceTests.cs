using System.Linq.Expressions;
using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Services;

namespace Application.UnitTests.Common.Services;

/// <summary>
///     Tests for the <see cref="QueryService{T}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class QueryServiceTests
{
    /// <summary>
    ///     The query service.
    /// </summary>
    private readonly IQueryService<string> _queryService;

    /// <summary>
    ///     Setups QueryServiceTests.
    /// </summary>
    public QueryServiceTests()
    {
        _queryService = new QueryService<string>();
    }

    /// <summary>
    ///     Tests that Filter method filters collection when given delegates.
    /// </summary>
    [Fact]
    public void Filter_ShouldFilterCollection_WhenGivenDelegates()
    {
        // Arrange
        var collection = new List<string> { "apple", "banana", "cherry", "date" }.AsQueryable();
        var delegates = new List<Expression<Func<string, bool>>>
        {
            x => x.StartsWith("a"),
            x => x.Length > 4
        };

        // Act
        var result = _queryService.Filter(collection, delegates);

        // Assert
        result.Should().HaveCount(1);
        result.First().Should().Be("apple");
    }

    /// <summary>
    ///     Tests that Filter method returns original collection when delegates list is empty.
    /// </summary>
    [Fact]
    public void Filter_ShouldReturnOriginalCollection_WhenDelegatesListIsEmpty()
    {
        // Arrange
        var collection = new List<string> { "apple", "banana", "cherry", "date" }.AsQueryable();

        // Act
        var result = _queryService.Filter(collection, new List<Expression<Func<string, bool>>>());

        // Assert
        result.Should().BeEquivalentTo(collection);
    }

    /// <summary>
    ///     Tests that Sort method sorts collection in ascending order when SortDirection is Asc.
    /// </summary>
    [Fact]
    public void Sort_ShouldSortCollectionInAscendingOrder_WhenSortDirectionIsAsc()
    {
        // Arrange
        var collection = new List<string> { "banana", "cherry", "apple", "date" }.AsQueryable();
        Expression<Func<string, object>> sortingDelegate = x => x.Length;

        // Act
        var result = _queryService.Sort(collection, sortingDelegate, SortDirection.Asc);

        // Assert
        result.Should().HaveCount(4);
        result.First().Should().Be("date");
        result.Last().Should().Be("cherry");
    }

    /// <summary>
    ///     Tests that Sort method sorts collection in descending order when SortDirection is Desc.
    /// </summary>
    [Fact]
    public void Sort_ShouldSortCollectionInDescendingOrder_WhenSortDirectionIsDesc()
    {
        // Arrange
        var collection = new List<string> { "banana", "cherry", "apple", "date" }.AsQueryable();
        Expression<Func<string, object>> sortingDelegate = x => x.Length;
        var queryService = new QueryService<string>();

        // Act
        var result = queryService.Sort(collection, sortingDelegate, SortDirection.Desc);

        // Assert
        result.Should().HaveCount(4);
        result.First().Should().Be("banana");
        result.Last().Should().Be("date");
    }
}