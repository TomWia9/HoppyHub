using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Beers;

namespace SharedUtilities.UnitTests.EventValidators.Beers;

/// <summary>
///     Unit tests for the <see cref="BeerOpinionChanged" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerOpinionChangedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly BeerOpinionChangedValidator _validator;

    /// <summary>
    ///     Setups BeerOpinionChangedValidatorTests.
    /// </summary>
    public BeerOpinionChangedValidatorTests()
    {
        _validator = new BeerOpinionChangedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for OpinionsCount when OpinionsCount is valid.
    /// </summary>
    [Fact]
    public void BeerOpinionChanged_ShouldNotHaveValidationErrorForOpinionsCount_WhenOpinionsCountIsValid()
    {
        // Arrange
        var beerOpinionChangedEvent = new BeerOpinionChanged
        {
            OpinionsCount = 1
        };

        // Act
        var result = _validator.TestValidate(beerOpinionChangedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.OpinionsCount);
    }

    /// <summary>
    ///     Tests that validation should have error for OpinionsCount when OpinionsCount exceeds maximum length.
    /// </summary>
    [Fact]
    public void BeerOpinionChanged_ShouldHaveValidationErrorForOpinionsCount_WhenOpinionsCountExceedsMaximumLength()
    {
        // Arrange
        var beerOpinionChangedEvent = new BeerOpinionChanged
        {
            OpinionsCount = -1
        };

        // Act
        var result = _validator.TestValidate(beerOpinionChangedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OpinionsCount);
    }

    /// <summary>
    ///     Tests that validation should not have error for NewBeerRating when NewBeerRating is valid.
    /// </summary>
    [Fact]
    public void BeerOpinionChanged_ShouldNotHaveValidationErrorForNewBeerRating_WhenNewBeerRatingIsValid()
    {
        // Arrange
        var beerOpinionChangedEvent = new BeerOpinionChanged
        {
            NewBeerRating = 5.55
        };

        // Act
        var result = _validator.TestValidate(beerOpinionChangedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NewBeerRating);
    }

    /// <summary>
    ///     Tests that validation should have error for NewBeerRating when NewBeerRating is empty.
    /// </summary>
    [Fact]
    public void BeerOpinionChanged_ShouldHaveValidationErrorForNewBeerRating_WhenNewBeerRatingIsEmpty()
    {
        // Arrange
        var beerOpinionChangedEvent = new BeerOpinionChanged
        {
            NewBeerRating = -1
        };

        // Act
        var result = _validator.TestValidate(beerOpinionChangedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewBeerRating);
    }
}