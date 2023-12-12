using System.Linq.Expressions;
using Moq;
using SharedUtilities.Abstractions;

namespace SharedUtilities.UnitTests.Abstractions;

/// <summary>
///     Tests for the <see cref="FilteringHelperBase{T, TRequest}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class FilteringHelperBaseTests
{
    /// <summary>
    ///     Tests that GetSortingColumn method returns first sorting column when SortBy is null or empty.
    /// </summary>
    /// <param name="sortBy">The sort by</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetSortingColumn_ShouldReturnFirstSortingColumn_WhenSortByIsNullOrEmpty(string? sortBy)
    {
        // Arrange
        var sortingColumns = new Dictionary<string, Expression<Func<object, object>>>
        {
            { "COLUMN1", x => x },
            { "COLUMN2", x => x },
        };
        var filteringHelper = new Mock<FilteringHelperBase<object, object>>(sortingColumns)
            .Object;

        // Act
        var sortingColumn = filteringHelper.GetSortingColumn(sortBy);

        // Assert
        sortingColumn.Should().BeSameAs(sortingColumns["COLUMN1"]);
    }

    /// <summary>
    ///     Tests that GetSortingColumn method returns correct sorting column when SortBy is specified.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnCorrectSortingColumn_WhenSortByIsSpecified()
    {
        // Arrange
        var sortingColumns = new Dictionary<string, Expression<Func<object, object>>>
        {
            { "COLUMN1", x => x },
            { "COLUMN2", x => x }
        };
        var filteringHelper = new Mock<FilteringHelperBase<object, object>>(sortingColumns)
            .Object;

        // Act
        var sortingColumn = filteringHelper.GetSortingColumn("COLUMN2");

        // Assert
        sortingColumn.Should().BeSameAs(sortingColumns["COLUMN2"]);
    }
}