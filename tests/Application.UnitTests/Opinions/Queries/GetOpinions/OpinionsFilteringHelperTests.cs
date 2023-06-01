using Application.Opinions.Queries.GetOpinions;
using Domain.Entities;

namespace Application.UnitTests.Opinions.Queries.GetOpinions;

/// <summary>
///     Tests for the <see cref="OpinionsFilteringHelper"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class OpinionsFilteringHelperTests
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
        var result = OpinionsFilteringHelper.GetSortingColumn(sortBy);

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
        var result = OpinionsFilteringHelper.GetSortingColumn(sortBy);

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
            BeerId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            SearchQuery = "test"
        };

        // Act
        var result = OpinionsFilteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(4, "Min and Max are merged into single delegate");
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates without SearchQuery.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegatesWithoutSearchQuery()
    {
        // Arrange
        var request = new GetOpinionsQuery()
        {
            MinRating = 5,
            MaxRating = 10,
            BeerId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };

        // Act
        var result = OpinionsFilteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(3, "Min and Max are merged into single delegate");
    }
}