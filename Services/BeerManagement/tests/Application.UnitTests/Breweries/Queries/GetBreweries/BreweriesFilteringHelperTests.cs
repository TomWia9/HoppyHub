using Application.Breweries.Queries.GetBreweries;
using Domain.Entities;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Breweries.Queries.GetBreweries;

/// <summary>
///     Tests for the <see cref="BreweriesFilteringHelper" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BreweriesFilteringHelperTests
{
    /// <summary>
    ///     The breweries filtering helper.
    /// </summary>
    private readonly IFilteringHelper<Brewery, GetBreweriesQuery> _filteringHelper;

    /// <summary>
    ///     Setups BreweriesFilteringHelperTests.
    /// </summary>
    public BreweriesFilteringHelperTests()
    {
        _filteringHelper = new BreweriesFilteringHelper();
    }

    /// <summary>
    ///     Tests that GetSortingColumn method returns first column when SortBy is null.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnFirstColumn_WhenSortByIsNull()
    {
        // Arrange
        string? sortBy = null;

        // Act
        var result = _filteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(BreweriesFilteringHelper.SortingColumns.First().Value);
    }

    /// <summary>
    ///     Tests that GetSortingColumn method returns correct column when SortBy is provided.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnCorrectColumn_WhenSortByIsProvided()
    {
        // Arrange
        const string sortBy = nameof(Brewery.FoundationYear);

        // Act
        var result = _filteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(BreweriesFilteringHelper.SortingColumns[sortBy.ToUpper()]);
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegates()
    {
        // Arrange
        var request = new GetBreweriesQuery
        {
            Name = "Pinta",
            Country = "Poland",
            State = "Śląskie",
            City = "Wieprz",
            MinFoundationYear = 2000,
            MaxFoundationYear = 2023,
            SearchQuery = "IPA"
        };

        // Act
        var result = _filteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(6, "Min and Max are merged into single delegate");
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates without SearchQuery.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegatesWithoutSearchQuery()
    {
        // Arrange
        var request = new GetBreweriesQuery
        {
            Name = "Pinta",
            Country = "Poland",
            State = "Śląskie",
            City = "Wieprz",
            MinFoundationYear = 2000,
            MaxFoundationYear = 2023
        };

        // Act
        var result = _filteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(5, "Min and Max are merged into single delegate");
    }
}