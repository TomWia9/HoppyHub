using Application.Beers.Queries.GetBeers;
using Domain.Entities;

namespace Application.UnitTests.Beers.Queries.GetBeers;

/// <summary>
///     Tests for the <see cref="BeersFilteringHelper"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeersFilteringHelperTests
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
        var result = BeersFilteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(BeersFilteringHelper.SortingColumns.First().Value);
    }

    /// <summary>
    ///     Tests that GetSortingColumn method returns correct column when SortBy is provided.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnCorrectColumn_WhenSortByIsProvided()
    {
        // Arrange
        const string sortBy = nameof(Beer.Name);

        // Act
        var result = BeersFilteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(BeersFilteringHelper.SortingColumns[sortBy.ToUpper()]);
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegates()
    {
        // Arrange
        var request = new GetBeersQuery
        {
            MinAlcoholByVolume = 5,
            MaxAlcoholByVolume = 10,
            MinExtract = 10,
            MaxExtract = 18,
            MinIbu = 20,
            MaxIbu = 50,
            Name = "IPA",
            BreweryId = Guid.NewGuid(),
            BeerStyleId = Guid.NewGuid(),
            SearchQuery = "IPA"
        };

        // Act
        var result = BeersFilteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(7, "Min and Max are merged into single delegate");
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates without SearchQuery.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegatesWithoutSearchQuery()
    {
        // Arrange
        var request = new GetBeersQuery
        {
            MinAlcoholByVolume = 5,
            MaxAlcoholByVolume = 10,
            MinExtract = 10,
            MaxExtract = 18,
            MinIbu = 20,
            MaxIbu = 50,
            Name = "IPA",
            BreweryId = Guid.NewGuid(),
            BeerStyleId = Guid.NewGuid()
        };

        // Act
        var result = BeersFilteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(6, "Min and Max are merged into single delegate");
    }
}