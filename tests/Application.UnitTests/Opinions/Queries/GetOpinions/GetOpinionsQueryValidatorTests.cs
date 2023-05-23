using Application.Opinions.Queries.GetOpinions;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Opinions.Queries.GetOpinions;

/// <summary>
///     Unit tests for the <see cref="GetOpinionsQueryValidator"/> class.
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
    ///     Tests that validation should not have error for MinRate when MinRate is valid.
    /// </summary>
    [Fact]
    public void GetOpinionsQuery_ShouldNotHaveValidationErrorForMinRate_WhenMinRateIsValid()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MinRate = 4
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinRate);
    }

    /// <summary>
    ///     Tests that validation should have error for MinRate when MinRate is out of range.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void GetOpinionsQuery_ShouldHaveValidationErrorForMinRate_WhenMinRateIsOutOfRange(
        int minRate)
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MinRate = minRate
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinRate);
    }

    /// <summary>
    ///     Tests that validation should have error for MinRate when MinRate is greater than MaxRate.
    /// </summary>
    [Fact]
    public void
        GetOpinionsQuery_ShouldHaveValidationErrorForMinAlcoholByVolume_WhenMinAlcoholByVolumeIsGreaterThanMaxAlcoholByVolume()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MinRate = 4,
            MaxRate = 3
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinRate)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxRate when MaxRate is valid.
    /// </summary>
    [Fact]
    public void GetOpinionsQuery_ShouldNotHaveValidationErrorForMaxRate_WhenMaxRateIsValid()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MaxRate = 6
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxRate);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxRate when MaxRate is out of range.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void GetOpinionsQuery_ShouldHaveValidationErrorForMaxRate_WhenMaxRateIsOutOfRange(
        int maxRate)
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MaxRate = maxRate
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxRate);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxAlcoholByVolume when MaxRate is less than MaxRate.
    /// </summary>
    [Fact]
    public void
        GetOpinionsQuery_ShouldHaveValidationErrorForMaxAlcoholByVolume_WhenMaxAlcoholByVolumeIsLessThanMinAlcoholByVolume()
    {
        // Arrange
        var query = new GetOpinionsQuery
        {
            MinRate = 4,
            MaxRate = 3
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxRate)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for SortBy when SortBy is valid.
    /// </summary>
    [Theory]
    [InlineData("lastModified")]
    [InlineData("rate")]
    [InlineData("comment")]
    [InlineData("")]
    [InlineData(null)]
    public void GetOpinionsQuery_ShouldNotHaveValidationErrorForSortBy_WhenSortByIsAllowedColumn(string sortBy)
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
            .WithErrorMessage("SortBy must be in [LASTMODIFIED, RATE, COMMENT]");
    }
}