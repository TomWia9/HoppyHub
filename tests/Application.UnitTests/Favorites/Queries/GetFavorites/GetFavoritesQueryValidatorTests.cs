using Application.Favorites.Queries.GetFavorites;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Favorites.Queries.GetFavorites;

/// <summary>
///     Unit tests for the <see cref="GetFavoritesQueryValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetFavoritesQueryValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly GetFavoritesQueryValidator _validator;

    /// <summary>
    ///     Setups GetOpinionsQueryValidatorTests.
    /// </summary>
    public GetFavoritesQueryValidatorTests()
    {
        _validator = new GetFavoritesQueryValidator();
    }
    
    /// <summary>
    ///     Tests that validation should not have error for UserId when UserId is valid.
    /// </summary>
    [Fact]
    public void GetFavoritesQuery_ShouldNotHaveValidationErrorForUserId_WhenUserIdIsValid()
    {
        // Arrange
        var command = new GetFavoritesQuery
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    /// <summary>
    ///     Tests that validation should have error for UserId when UserId is empty.
    /// </summary>
    [Fact]
    public async Task GetFavoritesQuery_ShouldHaveValidationErrorForUserId_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new GetFavoritesQuery
        {
            UserId = Guid.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
    
    /// <summary>
    ///     Tests that validation should not have error for SortBy when SortBy is valid.
    /// </summary>
    [Theory]
    [InlineData("lastModified")]
    [InlineData("Beer")]
    [InlineData("")]
    [InlineData(null)]
    public void GetFavoritesQuery_ShouldNotHaveValidationErrorForSortBy_WhenSortByIsAllowedColumn(string sortBy)
    {
        // Arrange
        var query = new GetFavoritesQuery
        {
            SortBy = sortBy
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    /// <summary>
    ///     Tests that validation should have error for SortBy when SortBy is not allowed column.
    /// </summary>
    [Fact]
    public void GetFavoritesQuery_ShouldHaveValidationErrorForSortBy_WhenSortByIsNotAllowedColumn()
    {
        // Arrange
        var query = new GetFavoritesQuery
        {
            SortBy = "invalid_column"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("SortBy must be in [LASTMODIFIED, BEER]");
    }
}