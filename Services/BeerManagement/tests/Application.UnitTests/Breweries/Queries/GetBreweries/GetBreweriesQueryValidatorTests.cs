using Application.Breweries.Dtos;
using Application.Breweries.Queries.GetBreweries;
using FluentValidation.TestHelper;
using Moq;

namespace Application.UnitTests.Breweries.Queries.GetBreweries;

/// <summary>
///     Unit tests for the <see cref="GetBreweriesQueryValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBreweriesQueryValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly GetBreweriesQueryValidator _validator;

    /// <summary>
    ///     Setups GetBreweriesQueryValidatorTests.
    /// </summary>
    public GetBreweriesQueryValidatorTests()
    {
        Mock<TimeProvider> timeProviderMock = new();
        timeProviderMock.Setup(x => x.GetUtcNow()).Returns(new DateTime(2023, 3, 26));
        _validator = new GetBreweriesQueryValidator(timeProviderMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            Name = "Test Name"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name exceeds maximum length.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            Name = new string('x', 501)
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should not have error for Country when Country is valid.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldNotHaveValidationErrorForCountry_WhenCountryIsValid()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            Country = "Test Country"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should have error for Country when Country exceeds maximum length.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldHaveValidationErrorForCountry_WhenCountryExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            Country = new string('x', 51)
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should not have error for State when State is valid.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldNotHaveValidationErrorForState_WhenStateIsValid()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            State = "Test State"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.State);
    }

    /// <summary>
    ///     Tests that validation should have error for State when State exceeds maximum length.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldHaveValidationErrorForState_WhenStateExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            State = new string('x', 51)
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    /// <summary>
    ///     Tests that validation should not have error for City when City is valid.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldNotHaveValidationErrorForCity_WhenCityIsValid()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            City = "Test City"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    ///     Tests that validation should have error for City when City exceeds maximum length.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldHaveValidationErrorForCity_WhenCityExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            City = new string('x', 51)
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    ///     Tests that validation should not have error for MinFoundationYear when MinFoundationYear is valid.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldNotHaveValidationErrorForMinFoundationYear_WhenMinFoundationYearIsValid()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            MinFoundationYear = 2010
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinFoundationYear);
    }

    /// <summary>
    ///     Tests that validation should have error for MinFoundationYear when MinFoundationYear is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9999)]
    public void GetBreweriesQuery_ShouldHaveValidationErrorForMinFoundationYear_WhenMinFoundationYearIsOutOfRange(
        double minFoundationYear)
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            MinFoundationYear = minFoundationYear
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinFoundationYear);
    }

    /// <summary>
    ///     Tests that validation should have error for MinFoundationYear when MinFoundationYear is greater than
    ///     MaxFoundationYear.
    /// </summary>
    [Fact]
    public void
        GetBreweriesQuery_ShouldHaveValidationErrorForMinFoundationYear_WhenMinFoundationYearIsGreaterThanMaxFoundationYear()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            MinFoundationYear = 2012,
            MaxFoundationYear = 2000
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinFoundationYear)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxFoundationYear when MaxFoundationYear is valid.
    /// </summary>
    [Fact]
    public void GetBreweriesQuery_ShouldNotHaveValidationErrorForMaxFoundationYear_WhenMaxFoundationYearIsValid()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            MaxFoundationYear = 2020
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxFoundationYear);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxFoundationYear when MaxFoundationYear is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9999)]
    public void GetBreweriesQuery_ShouldHaveValidationErrorForMaxFoundationYear_WhenMaxFoundationYearIsOutOfRange(
        double maxFoundationYear)
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            MaxFoundationYear = maxFoundationYear
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxFoundationYear);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxFoundationYear when MaxFoundationYear is less than
    ///     MinFoundationYear.
    /// </summary>
    [Fact]
    public void
        GetBreweriesQuery_ShouldHaveValidationErrorForMaxFoundationYear_WhenMaxFoundationYearIsLessThanMinFoundationYear()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            MinFoundationYear = 2004,
            MaxFoundationYear = 2002
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxFoundationYear)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for SortBy when SortBy is valid.
    /// </summary>
    [Theory]
    [InlineData(nameof(BreweryDto.Name))]
    [InlineData(nameof(BreweryDto.FoundationYear))]
    [InlineData("")]
    [InlineData(null)]
    public void GetBreweriesQuery_ShouldNotHaveValidationErrorForSortBy_WhenSortByIsAllowedColumn(string? sortBy)
    {
        // Arrange
        var query = new GetBreweriesQuery
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
    public void GetBreweriesQuery_ShouldHaveValidationErrorForSortBy_WhenSortByIsNotAllowedColumn()
    {
        // Arrange
        var query = new GetBreweriesQuery
        {
            SortBy = "invalid_column"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("SortBy must be in [NAME, FOUNDATIONYEAR]");
    }
}