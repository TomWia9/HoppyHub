using Application.Opinions.Dtos;
using Application.Opinions.Queries.GetOpinions;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Opinions.Queries.GetOpinions;

/// <summary>
///     Unit tests for the <see cref="GetOpinionsQueryValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetOpinionsQueryValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly GetOpinionsQueryValidator _validator;

    /// <summary>
    ///     Setups GetOpinionsQueryValidatorTests.
    /// </summary>
    public GetOpinionsQueryValidatorTests()
    {
        _validator = new GetOpinionsQueryValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for MinRating when MinRating is valid.
    /// </summary>
    [Fact]
    public void GetOpinionsQuery_ShouldNotHaveValidationErrorForMinRating_WhenMinRateIsValid()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MinRating = 4
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinRating);
    }

    /// <summary>
    ///     Tests that validation should have error for MinRating when MinRating is out of range.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void GetOpinionsQuery_ShouldHaveValidationErrorForMinRating_WhenMinRatingIsOutOfRange(
        int minRating)
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MinRating = minRating
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinRating);
    }

    /// <summary>
    ///     Tests that validation should have error for MinRating when MinRating is greater than MaxRating.
    /// </summary>
    [Fact]
    public void
        GetOpinionsQuery_ShouldHaveValidationErrorForMinRating_WhenMinRatingIsGreaterThanMaxRating()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MinRating = 4,
            MaxRating = 3
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinRating)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxRating when MaxRating is valid.
    /// </summary>
    [Fact]
    public void GetOpinionsQuery_ShouldNotHaveValidationErrorForMaxRating_WhenMaxRatingIsValid()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MaxRating = 6
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxRating);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxRating when MaxRating is out of range.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void GetOpinionsQuery_ShouldHaveValidationErrorForMaxRating_WhenMaxRatingIsOutOfRange(
        int maxRate)
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MaxRating = maxRate
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxRating);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxRating when MaxRating is less than MinRating.
    /// </summary>
    [Fact]
    public void
        GetOpinionsQuery_ShouldHaveValidationErrorForMaxRating_WhenMaxRatingIsLessThanMinRating()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MinRating = 4,
            MaxRating = 3
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxRating)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for SortBy when SortBy is valid.
    /// </summary>
    [Theory]
    [InlineData(nameof(OpinionDto.LastModified))]
    [InlineData(nameof(OpinionDto.Rating))]
    [InlineData(nameof(OpinionDto.Comment))]
    [InlineData("")]
    [InlineData(null)]
    public void GetOpinionsQuery_ShouldNotHaveValidationErrorForSortBy_WhenSortByIsAllowedColumn(string? sortBy)
    {
        // Arrange
        var query = new GetOpinionsQuery
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
    public void GetOpinionsQuery_ShouldHaveValidationErrorForSortBy_WhenSortByIsNotAllowedColumn()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            SortBy = "invalid_column"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("SortBy must be in [LASTMODIFIED, RATING, COMMENT]");
    }
}