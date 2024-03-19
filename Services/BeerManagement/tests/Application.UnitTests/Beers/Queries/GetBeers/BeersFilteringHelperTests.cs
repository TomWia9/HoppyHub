using Application.Beers.Queries.GetBeers;
using Domain.Entities;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Beers.Queries.GetBeers;

/// <summary>
///     Tests for the <see cref="BeersFilteringHelper" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeersFilteringHelperTests
{
    /// <summary>
    ///     The beers filtering helper.
    /// </summary>
    private readonly IFilteringHelper<Beer, GetBeersQuery> _filteringHelper;

    /// <summary>
    ///     Setups BeersFilteringHelperTests.
    /// </summary>
    public BeersFilteringHelperTests()
    {
        _filteringHelper = new BeersFilteringHelper();
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
        var result = _filteringHelper.GetSortingColumn(sortBy);

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
            MinReleaseDate = DateOnly.MinValue.ToString(),
            MaxReleaseDate = DateOnly.FromDateTime(DateTime.Now).ToString(),
            Name = "IPA",
            BreweryId = Guid.NewGuid(),
            BeerStyleId = Guid.NewGuid(),
            MinRating = 2,
            MaxRating = 8,
            MinOpinionsCount = 20,
            MaxOpinionsCount = 50,
            MinFavoritesCount = 20,
            MaxFavoritesCount = 50,
            SearchQuery = "IPA"
        };

        // Act
        var result = _filteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(11, "Min and Max are merged into single delegate");
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
            MinReleaseDate = DateOnly.MinValue.ToString(),
            MaxReleaseDate = DateOnly.FromDateTime(DateTime.Now).ToString(),
            Name = "IPA",
            BreweryId = Guid.NewGuid(),
            BeerStyleId = Guid.NewGuid(),
            MinRating = 2,
            MaxRating = 8,
            MinOpinionsCount = 20,
            MaxOpinionsCount = 50,
            MinFavoritesCount = 20,
            MaxFavoritesCount = 50
        };

        // Act
        var result = _filteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(10, "Min and Max are merged into single delegate");
    }
}