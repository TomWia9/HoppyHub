using Application.Beers.Dtos;
using Application.Beers.Queries.GetBeers;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Beers.Queries.GetBeers;

/// <summary>
///     Unit tests for the <see cref="GetBeersQueryValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBeersQueryValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly GetBeersQueryValidator _validator;

    /// <summary>
    ///     Setups GetBeersQueryValidatorTests.
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
    public void GetBeersQuery_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            Name = new string('x', 201)
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
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
    public void GetBeersQuery_ShouldHaveValidationErrorForMinAlcoholByVolume_WhenMinAlcoholByVolumeIsOutOfRange(
        double minAlcoholByVolume)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinAlcoholByVolume = minAlcoholByVolume
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinAlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should have error for MinAlcoholByVolume when MinAlcoholByVolume is greater than
    ///     MaxAlcoholByVolume.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMinAlcoholByVolume_WhenMinAlcoholByVolumeIsGreaterThanMaxAlcoholByVolume()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinAlcoholByVolume = 4,
            MaxAlcoholByVolume = 3
        };

        // Act
        var result = _validator.TestValidate(query);

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
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxAlcoholByVolume_WhenMaxAlcoholByVolumeIsOutOfRange(
        double maxAlcoholByVolume)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxAlcoholByVolume = maxAlcoholByVolume
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxAlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxAlcoholByVolume when MaxAlcoholByVolume is less than
    ///     MinAlcoholByVolume.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMaxAlcoholByVolume_WhenMaxAlcoholByVolumeIsLessThanMinAlcoholByVolume()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinAlcoholByVolume = 4,
            MaxAlcoholByVolume = 3
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxAlcoholByVolume)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MinExtract when MinExtract is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMinExtract_WhenMinExtractIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinExtract = 10
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinExtract);
    }

    /// <summary>
    ///     Tests that validation should have error for MinExtract when MinExtract is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void GetBeersQuery_ShouldHaveValidationErrorForMinExtract_WhenMinExtractIsOutOfRange(
        double minExtract)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinExtract = minExtract
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinExtract);
    }

    /// <summary>
    ///     Tests that validation should have error for MinExtract when MinExtract is greater than MaxExtract.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMinExtract_WhenMinExtractIsGreaterThanMaxExtract()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinExtract = 4,
            MaxExtract = 3
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinExtract)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxExtract when MaxExtract is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMaxExtract_WhenMaxExtractIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxExtract = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxExtract);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxExtract when MaxExtract is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxExtract_WhenMaxExtractIsOutOfRange(
        double maxExtract)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxExtract = maxExtract
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxExtract);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxExtract when MaxExtract is less than MinExtract.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMaxExtract_WhenMaxExtractIsLessThanMinExtract()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinExtract = 4,
            MaxExtract = 3
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxExtract)
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
    public void GetBeersQuery_ShouldHaveValidationErrorForMinIbu_WhenMinIbuIsOutOfRange(
        int minIbu)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinIbu = minIbu
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinIbu);
    }

    /// <summary>
    ///     Tests that validation should have error for MinIbu when MinIbu is greater than MaxIbu.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForMinIbu_WhenMinIbuIsGreaterThanMaxIbu()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinIbu = 30,
            MaxIbu = 20
        };

        // Act
        var result = _validator.TestValidate(query);

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
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxIbu_WhenMaxIbuIsOutOfRange(
        int maxIbu)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxIbu = maxIbu
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxIbu);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxIbu when MaxIbu is less than MinIbu.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxIbu_WhenMaxIbuIsLessThanMinIbu()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinIbu = 30,
            MaxIbu = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxIbu)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MinRating when MinRating is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMinRating_WhenMinRatingIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinRating = 7
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
    [InlineData(-1)]
    [InlineData(11)]
    public void GetBeersQuery_ShouldHaveValidationErrorForMinRating_WhenMinRatingIsOutOfRange(
        double minRating)
    {
        // Arrange
        var query = new GetBeersQuery
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
    public void GetBeersQuery_ShouldHaveValidationErrorForMinRating_WhenMinRatingIsGreaterThanMaxRating()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinRating = 30,
            MaxRating = 20
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
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMaxRating_WhenMaxRatingIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxRating = 7
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
    [InlineData(-1)]
    [InlineData(11)]
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxRating_WhenMaxRatingIsOutOfRange(
        double maxRating)
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxRating = maxRating
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
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxRating_WhenMaxRatingIsLessThanMinRating()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinRating = 30,
            MaxRating = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxRating)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MinOpinionsCount when MinOpinionsCount is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMinOpinionsCount_WhenMinOpinionsCountIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinOpinionsCount = 25
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinOpinionsCount);
    }

    /// <summary>
    ///     Tests that validation should have error for MinOpinionsCount when MinOpinionsCount is out of range.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForMinOpinionsCount_WhenMinOpinionsCountIsOutOfRange()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinOpinionsCount = -1
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinOpinionsCount);
    }

    /// <summary>
    ///     Tests that validation should have error for MinOpinionsCount when MinOpinionsCount is greater than
    ///     MaxOpinionsCount.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMinOpinionsCount_WhenMinOpinionsCountIsGreaterThanMaxOpinionsCount()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinOpinionsCount = 30,
            MaxOpinionsCount = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinOpinionsCount)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxOpinionsCount when MaxOpinionsCount is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMaxOpinionsCount_WhenMaxOpinionsCountIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxOpinionsCount = 25
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxRating);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxOpinionsCount when MaxOpinionsCount is out of range.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxOpinionsCount_WhenMaxOpinionsCountIsOutOfRange()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxOpinionsCount = -1
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxOpinionsCount);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxOpinionsCount when MaxOpinionsCount is less than MinRating.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMaxOpinionsCount_WhenMaxOpinionsCountIsLessThanMinOpinionsCount()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinOpinionsCount = 30,
            MaxOpinionsCount = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxOpinionsCount)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MinFavoritesCount when MinFavoritesCount is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMinFavoritesCount_WhenMinFavoritesCountIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinFavoritesCount = 25
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinFavoritesCount);
    }

    /// <summary>
    ///     Tests that validation should have error for MinFavoritesCount when MinFavoritesCount is out of range.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForMinFavoritesCount_WhenMinFavoritesCountIsOutOfRange()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinFavoritesCount = -1
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinFavoritesCount);
    }

    /// <summary>
    ///     Tests that validation should have error for MinFavoritesCount when MinFavoritesCount is greater than
    ///     MaxFavoritesCount.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMinFavoritesCount_WhenMinFavoritesCountIsGreaterThanMaxFavoritesCount()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinFavoritesCount = 30,
            MaxFavoritesCount = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinFavoritesCount)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxFavoritesCount when MaxFavoritesCount is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMaxFavoritesCount_WhenMaxFavoritesCountIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxFavoritesCount = 25
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxFavoritesCount);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxFavoritesCount when MaxFavoritesCount is out of range.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxFavoritesCount_WhenMaxFavoritesCountIsOutOfRange()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxFavoritesCount = -1
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxFavoritesCount);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxFavoritesCount when MaxFavoritesCount is less than
    ///     MinFavoritesCount.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMaxFavoritesCount_WhenMaxFavoritesCountIsLessThanMinFavoritesCount()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinFavoritesCount = 30,
            MaxFavoritesCount = 20
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxFavoritesCount)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }

    /// <summary>
    ///     Tests that validation should not have error for SortBy when SortBy is valid.
    /// </summary>
    [Theory]
    [InlineData(nameof(BeerDto.Name))]
    [InlineData(nameof(BeerDto.Brewery))]
    [InlineData(nameof(BeerDto.BeerStyle))]
    [InlineData(nameof(BeerDto.AlcoholByVolume))]
    [InlineData(nameof(BeerDto.Blg))]
    [InlineData(nameof(BeerDto.Ibu))]
    [InlineData(nameof(BeerDto.Rating))]
    [InlineData(nameof(BeerDto.OpinionsCount))]
    [InlineData(nameof(BeerDto.FavoritesCount))]
    [InlineData(nameof(BeerDto.ReleaseDate))]
    [InlineData("")]
    [InlineData(null)]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForSortBy_WhenSortByIsAllowedColumn(string? sortBy)
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
            .WithErrorMessage(
                "SortBy must be in [NAME, BREWERY, BEERSTYLE, ALCOHOLBYVOLUME, BLG, IBU, RATING, OPINIONSCOUNT, FAVORITESCOUNT, RELEASEDATE]");
    }

    /// <summary>
    ///     Tests that validation should not have error for MinReleaseDate when MinReleaseDate is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForFrom_WhenFromIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinReleaseDate = "01.01.20"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MinReleaseDate);
    }

    /// <summary>
    ///     Tests that validation should have error for MinReleaseDate when MinReleaseDate is invalid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForMinReleaseDate_WhenMinReleaseDateIsInvalid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinReleaseDate = "abc"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinReleaseDate);
    }

    /// <summary>
    ///     Tests that validation should have error for MinReleaseDate when MinReleaseDate is greater than MaxReleaseDate.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMinReleaseDate_WhenMinReleaseDateIsGreaterThanMaxReleaseDate()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinReleaseDate = "02.01.20",
            MaxReleaseDate = "01.01.20"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinReleaseDate)
            .WithErrorMessage("Min value must be less than or equal to Max value");
    }

    /// <summary>
    ///     Tests that validation should not have error for MaxReleaseDate when MaxReleaseDate is valid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldNotHaveValidationErrorForMaxReleaseDate_WhenMaxReleaseDateIsValid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxReleaseDate = DateOnly.FromDateTime(DateTime.Now).ToString()
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxReleaseDate);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxReleaseDate when MaxReleaseDate is invalid.
    /// </summary>
    [Fact]
    public void GetBeersQuery_ShouldHaveValidationErrorForMaxReleaseDate_WhenMaxReleaseDateIsInvalid()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MaxReleaseDate = "abc"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxReleaseDate);
    }

    /// <summary>
    ///     Tests that validation should have error for MaxReleaseDate when MaxReleaseDate is less than MinReleaseDate.
    /// </summary>
    [Fact]
    public void
        GetBeersQuery_ShouldHaveValidationErrorForMaxReleaseDate_WhenMaxReleaseDateIsLessThanMinReleaseDate()
    {
        // Arrange
        var query = new GetBeersQuery
        {
            MinReleaseDate = "02.01.20",
            MaxReleaseDate = "01.01.20"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxReleaseDate)
            .WithErrorMessage("Max value must be greater than or equal to Min value");
    }
}