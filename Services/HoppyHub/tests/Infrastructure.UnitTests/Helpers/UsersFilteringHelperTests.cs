using Application.Users.Queries.GetUsers;
using Infrastructure.Helpers;

namespace Infrastructure.UnitTests.Helpers;

/// <summary>
///     Tests for the <see cref="UsersFilteringHelper" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UsersFilteringHelperTests
{
    /// <summary>
    ///     Tests that GetSortingColumn method when SortBy is null returns first sorting column.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnFirstSortingColumn_WhenSortByIsNull()
    {
        // Arrange
        var expectedSortingColumn = UsersFilteringHelper.SortingColumns.First().Value;

        // Act
        var sortingColumn = UsersFilteringHelper.GetSortingColumn(null);

        // Assert
        sortingColumn.Should().Be(expectedSortingColumn);
    }

    /// <summary>
    ///     Tests that GetSortingColumn method when SortBy is valid returns sorting column.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnSortingColumn_WhenSortByIsValid()
    {
        // Arrange
        const string sortBy = "username";
        var expectedSortingColumn = UsersFilteringHelper.SortingColumns[sortBy.ToUpper()];

        // Act
        var sortingColumn = UsersFilteringHelper.GetSortingColumn(sortBy);

        // Assert
        sortingColumn.Should().Be(expectedSortingColumn);
    }

    /// <summary>
    ///     Tests that GetDelegates method when searchQuery is null or whitespace returns empty list.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void GetDelegates_ShouldReturnEmptyList_WhenSearchQueryIsNullOrWhiteSpace(string? searchQuery)
    {
        // Arrange
        var query = new GetUsersQuery { SearchQuery = searchQuery };

        // Act
        var delegates = UsersFilteringHelper.GetDelegates(query);

        // Assert
        delegates.Should().BeEmpty();
    }

    /// <summary>
    ///     Tests that GetDelegates method when searchQuery is valid returns not empty list.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnSearchDelegateWhenSearchQueryIsValid()
    {
        // Arrange
        var query = new GetUsersQuery { SearchQuery = "test" };

        // Act
        var delegates = UsersFilteringHelper.GetDelegates(query).ToList();

        // Assert
        delegates.Should().ContainSingle();
    }
}