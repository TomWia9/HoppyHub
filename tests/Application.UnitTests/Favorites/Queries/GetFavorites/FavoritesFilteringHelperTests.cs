﻿using Application.Favorites.Queries.GetFavorites;
using Domain.Entities;

namespace Application.UnitTests.Favorites.Queries.GetFavorites;

/// <summary>
///     Tests for the <see cref="FavoritesFilteringHelper"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class FavoritesFilteringHelperTests
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
        var result = FavoritesFilteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(FavoritesFilteringHelper.SortingColumns.First().Value);
    }

    /// <summary>
    ///     Tests that GetSortingColumn method returns correct column when SortBy is provided.
    /// </summary>
    [Fact]
    public void GetSortingColumn_ShouldReturnCorrectColumn_WhenSortByIsProvided()
    {
        // Arrange
        const string sortBy = nameof(Favorite.Beer);

        // Act
        var result = FavoritesFilteringHelper.GetSortingColumn(sortBy);

        // Assert
        result.Should().Be(FavoritesFilteringHelper.SortingColumns[sortBy.ToUpper()]);
    }

    /// <summary>
    ///     Tests that GetDelegates method returns delegates.
    /// </summary>
    [Fact]
    public void GetDelegates_ShouldReturnDelegates()
    {
        // Arrange
        var request = new GetFavoritesQuery
        {
            UserId = Guid.NewGuid(),
            SearchQuery = "test"
        };

        // Act
        var result = FavoritesFilteringHelper.GetDelegates(request);

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
        var request = new GetFavoritesQuery()
        {
            UserId = Guid.NewGuid(),
        };

        // Act
        var result = FavoritesFilteringHelper.GetDelegates(request);

        // Assert
        result.Should().HaveCount(1);
    }
}