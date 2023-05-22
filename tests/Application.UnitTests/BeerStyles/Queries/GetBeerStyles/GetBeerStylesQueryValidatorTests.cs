using Application.BeerStyles.Queries.GetBeerStyles;
using FluentValidation.TestHelper;

namespace Application.UnitTests.BeerStyles.Queries.GetBeerStyles;

/// <summary>
///     Unit tests for the <see cref="GetBeerStylesQueryValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBeerStylesQueryValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly GetBeerStylesQueryValidator _validator;

    /// <summary>
    ///     Setups GetBeerStylesQueryValidatorTests.
    /// </summary>
    public GetBeerStylesQueryValidatorTests()
    {
        _validator = new GetBeerStylesQueryValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for CountryOfOrigin when CountryOfOrigin is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginIsValid()
    {
        // Arrange
        var query = new GetBeerStylesQuery
        {
            CountryOfOrigin = "Poland"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CountryOfOrigin);
    }

    /// <summary>
    ///     Tests that validation should have error for CountryOfOrigin when CountryOfOrigin exceeds maximum length.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBeerStylesQuery
        {
            CountryOfOrigin = new string('x', 51)
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CountryOfOrigin);
    }

    /// <summary>
    ///     Tests that validation should not have error for SortBy when SortBy is valid.
    /// </summary>
    [Theory]
    [InlineData("name")]
    [InlineData("countryOfOrigin")]
    [InlineData("")]
    [InlineData(null)]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForSortBy_WhenSortByIsAllowedColumn(string sortBy)
    {
        // Arrange
        var query = new GetBeerStylesQuery
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
    public void GetBeersQuery_ShouldHaveValidationErrorForSortBy_WhenSortByIsNotAllowedColumn()
    {
        // Arrange
        var query = new GetBeerStylesQuery
        {
            SortBy = "invalid_column"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("SortBy must be in [NAME, COUNTRYOFORIGIN]");
    }
}