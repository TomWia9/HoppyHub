﻿using Application.Beers.Queries.GetBeers;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Beers.Queries.GetBeers;

/// <summary>
///     Unit tests for the <see cref="GetBeersQueryValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBeersQueryValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly GetBeersQueryValidator _validator;

    /// <summary>
    ///     Setups CreateBeerCommandValidatorTests.
    /// </summary>
    public GetBeersQueryValidatorTests()
    {
        _validator = new GetBeersQueryValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
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
    public async Task GetBeersQuery_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            Name = new string('x', 201)
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should not have error for Brewery when Brewery is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForBrewery_WhenBreweryIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            Brewery = "Test Brewery"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Brewery);
    }

    /// <summary>
    ///     Tests that validation should have error for Brewery when Brewery exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task GetBeersQuery_ShouldHaveValidationErrorForBrewery_WhenBreweryExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            Brewery = new string('x', 201)
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Brewery);
    }

    /// <summary>
    ///     Tests that validation should not have error for Style when Style is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForStyle_WhenStyleIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            Style = "Test Style"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Style);
    }

    /// <summary>
    ///     Tests that validation should have error for Style when Style exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task GetBeersQuery_ShouldHaveValidationErrorForStyle_WhenStyleExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            Style = new string('x', 201)
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Style);
    }

    /// <summary>
    ///     Tests that validation should not have error for Country when Country is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForCountry_WhenCountryIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
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
    public async Task GetBeersQuery_ShouldHaveValidationErrorForCountry_WhenCountryExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            Country = new string('x', 201)
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should not have error for MinAlcoholByVolume when MinAlcoholByVolume is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMinAlcoholByVolume_WhenMinAlcoholByVolumeIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinAlcoholByVolume = 4.4
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinAlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should have error for MinAlcoholByVolume when MinAlcoholByVolume is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task GetBeersQuery_ShouldHaveValidationErrorForMinAlcoholByVolume_WhenMinAlcoholByVolumeIsOutOfRange(
        double minAlcoholByVolume)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinAlcoholByVolume = minAlcoholByVolume
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinAlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should have error for MinAlcoholByVolume when MinAlcoholByVolume is greater than MaxAlcoholByVolume.
    /// </summary>
    [Fact]
    public async Task
        GetBeersQuery_ShouldHaveValidationErrorForMinAlcoholByVolume_WhenMinAlcoholByVolumeIsGreaterThanMaxAlcoholByVolume()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinAlcoholByVolume = 4,
            MaxAlcoholByVolume = 3
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinAlcoholByVolume)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxAlcoholByVolume when MaxAlcoholByVolume is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMaxAlcoholByVolume_WhenMaxAlcoholByVolumeIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxAlcoholByVolume = 4.4
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxAlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxAlcoholByVolume when MaxAlcoholByVolume is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task GetBeersQuery_ShouldHaveValidationErrorForMaxAlcoholByVolume_WhenMaxAlcoholByVolumeIsOutOfRange(
        double maxAlcoholByVolume)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxAlcoholByVolume = maxAlcoholByVolume
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxAlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxAlcoholByVolume when MaxAlcoholByVolume is less than MinAlcoholByVolume.
    /// </summary>
    [Fact]
    public async Task
        GetBeersQuery_ShouldHaveValidationErrorForMaxAlcoholByVolume_WhenMaxAlcoholByVolumeIsLessThanMinAlcoholByVolume()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinAlcoholByVolume = 4,
            MaxAlcoholByVolume = 3
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxAlcoholByVolume)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MinIbu when MinIbu is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMinIbu_WhenMinIbuIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinIbu = 25
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinIbu);
    }

    /// <summary>
    ///     Tests that validation should have error for MinIbu when MinIbu is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(201)]
    public async Task GetBeersQuery_ShouldHaveValidationErrorForMinIbu_WhenMinIbuIsOutOfRange(
        int minIbu)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinIbu = minIbu
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinIbu);
    }

    /// <summary>
    ///     Tests that validation should have error for MinIbu when MinIbu is greater than MaxIbu.
    /// </summary>
    [Fact]
    public async Task
        GetBeersQuery_ShouldHaveValidationErrorForMinIbu_WhenMinIbuIsGreaterThanMaxIbu()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinIbu = 30,
            MaxIbu = 20
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinIbu)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxIbu when MaxIbu is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMaxIbu_WhenMaxIbuIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxIbu = 25
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxIbu);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxIbu when MaxIbu is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(201)]
    public async Task GetBeersQuery_ShouldHaveValidationErrorForMaxIbu_WhenMaxIbuIsOutOfRange(
        int maxIbu)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxIbu = maxIbu
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxIbu);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxIbu when MaxIbu is less than MinIbu.
    /// </summary>
    [Fact]
    public async Task
        GetBeersQuery_ShouldHaveValidationErrorForMaxIbu_WhenMaxIbuIsLessThanMinIbu()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinIbu = 30,
            MaxIbu = 20
        };

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxIbu)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for SortBy when SortBy is valid.
    /// </summary>
    [Theory]
    [InlineData("name")]
    [InlineData("brewery")]
    [InlineData("style")]
    [InlineData("COUNTRY")]
    [InlineData("ALCOHOLBYVOLUME")]
    [InlineData("blg")]
    [InlineData("plato")]
    [InlineData("ibu")]
    [InlineData("")]
    [InlineData(null)]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForSortBy_WhenSortByIsAllowedColumn(string sortBy)
    {
        // Arrange
        var query = new GetBeersQuery
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
        var query = new GetBeersQuery
        {
            SortBy = "invalid_column"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("SortBy must be in [NAME, BREWERY, STYLE, COUNTRY, ALCOHOLBYVOLUME, BLG, PLATO, IBU]");
    }
}