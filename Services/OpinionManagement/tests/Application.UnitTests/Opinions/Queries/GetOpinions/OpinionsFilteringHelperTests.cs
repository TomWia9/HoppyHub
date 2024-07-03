using Application.Opinions.Queries.GetOpinions;
using Domain.Entities;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Opinions.Queries.GetOpinions;

/// <summary>
///     Tests for the <see cref="OpinionsFilteringHelper" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class OpinionsFilteringHelperTests
{
    /// <summary>
    ///     The opinions filtering helper.
    /// </summary>
    private readonly IFilteringHelper<Opinion, GetOpinionsQuery> _filteringHelper;

    /// <summary>
    ///     Setups OpinionsFilteringHelperTests.
    /// </summary>
    public OpinionsFilteringHelperTests()
    {
        _filteringHelper = new OpinionsFilteringHelper();
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
        result.Should().Be(OpinionsFilteringHelper.SortingColumns.First().Value);
    }

    /// <summary>
    ///     Tests that GetSortingColumn method returns correct column when SortBy is provided.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnCorrectColumn_WhenSortByIsProvided()
    {
        // Arrange
        const string sortBy = nameof(Opinion.Rating);

        // Act
        var result = _filteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(OpinionsFilteringHelper.SortingColumns[sortBy.ToUpper()]);
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegates()
    {
        // Arrange
        var request = new GetOpinionsQuery
        {
            MinRating = 5,
            MaxRating = 10,
            From = DateTimeOffset.MinValue,
            To = DateTimeOffset.Now,
            BeerId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            HaveImages = true,
            SearchQuery = "test"
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
        var request = new GetOpinionsQuery
        {
            MinRating = 5,
            MaxRating = 10,
            From = DateTimeOffset.MinValue,
            To = DateTimeOffset.Now,
            BeerId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            HaveImages = false
        };

        // Act
        var result = _filteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(5, "Min and Max are merged into single delegate");
    }
}