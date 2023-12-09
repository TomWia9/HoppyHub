using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Beers;

namespace SharedUtilities.UnitTests.EventValidators.Beers;

/// <summary>
///     Unit tests for the <see cref="BeerFavoritesCountChanged" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerFavoritesCountChangedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly BeerFavoritesCountChangedValidator _validator;

    /// <summary>
    ///     Setups FavoritesCountChangedValidatorTests.
    /// </summary>
    public BeerFavoritesCountChangedValidatorTests()
    {
        _validator = new BeerFavoritesCountChangedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for FavoritesCount when FavoritesCount is valid.
    /// </summary>
    [Fact]
    public void BeerFavoritesCountChanged_ShouldNotHaveValidationErrorForFavoritesCount_WhenFavoritesCountIsValid()
    {
        // Arrange
        var favoritesCountChangedEvent = new BeerFavoritesCountChanged
        {
            FavoritesCount = 1
        };

        // Act
        var result = _validator.TestValidate(favoritesCountChangedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FavoritesCount);
    }

    /// <summary>
    ///     Tests that validation should have error for FavoritesCount when FavoritesCount exceeds maximum length.
    /// </summary>
    [Fact]
    public void
        BeerFavoritesCountChanged_ShouldHaveValidationErrorForFavoritesCount_WhenFavoritesCountExceedsMaximumLength()
    {
        // Arrange
        var favoritesCountChangedEvent = new BeerFavoritesCountChanged
        {
            FavoritesCount = -1
        };

        // Act
        var result = _validator.TestValidate(favoritesCountChangedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FavoritesCount);
    }
}