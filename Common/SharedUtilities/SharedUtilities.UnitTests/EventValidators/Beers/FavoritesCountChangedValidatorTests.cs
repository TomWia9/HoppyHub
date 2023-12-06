using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Beers;

namespace SharedUtilities.UnitTests.EventValidators.Beers;

/// <summary>
///     Unit tests for the <see cref="FavoritesCountChanged" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class FavoritesCountChangedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly FavoritesCountChangedValidator _validator;

    /// <summary>
    ///     Setups FavoritesCountChangedValidatorTests.
    /// </summary>
    public FavoritesCountChangedValidatorTests()
    {
        _validator = new FavoritesCountChangedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for FavoritesCount when FavoritesCount is valid.
    /// </summary>
    [Fact]
    public void FavoritesCountChanged_ShouldNotHaveValidationErrorForFavoritesCount_WhenFavoritesCountIsValid()
    {
        // Arrange
        var favoritesCountChangedEvent = new FavoritesCountChanged
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
        FavoritesCountChanged_ShouldHaveValidationErrorForFavoritesCount_WhenFavoritesCountExceedsMaximumLength()
    {
        // Arrange
        var favoritesCountChangedEvent = new FavoritesCountChanged
        {
            FavoritesCount = -1
        };

        // Act
        var result = _validator.TestValidate(favoritesCountChangedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FavoritesCount);
    }
}