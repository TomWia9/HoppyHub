using Application.BeerStyles.Queries.GetBeerStyles;
using Domain.Entities;

namespace Application.UnitTests.BeerStyles.Queries.GetBeerStyles;

/// <summary>
///     Tests for the <see cref="BeerStylesFilteringHelper"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerStylesFilteringHelperTests
{
    /// <summary>
    ///     Tests that GetSortingColumn method returns first column when SortBy is null.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnFirstColumn_WhenSortByIsNull()
    {
        // Arrange
        string? sortBy = null;

        // Act
        var result = BeerStylesFilteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(BeerStylesFilteringHelper.SortingColumns.First().Value);
    }

    /// <summary>
    ///     Tests that GetSortingColumn method returns correct column when SortBy is provided.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnCorrectColumn_WhenSortByIsProvided()
    {
        // Arrange
        const string sortBy = nameof(BeerStyle.CountryOfOrigin);

        // Act
        var result = BeerStylesFilteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(BeerStylesFilteringHelper.SortingColumns[sortBy.ToUpper()]);
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegates()
    {
        // Arrange
        var request = new GetBeerStylesQuery
        {
            CountryOfOrigin = "United States",
            SearchQuery = "IPA"
        };

        // Act
        var result = BeerStylesFilteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(2);
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates without SearchQuery.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegatesWithoutSearchQuery()
    {
        // Arrange
        var request = new GetBeerStylesQuery
        {
            CountryOfOrigin = "United States",
        };

        // Act
        var result = BeerStylesFilteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(1);
    }
}