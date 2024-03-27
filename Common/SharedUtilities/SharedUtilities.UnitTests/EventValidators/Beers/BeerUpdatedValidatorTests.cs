using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Beers;

namespace SharedUtilities.UnitTests.EventValidators.Beers;

/// <summary>
///     Unit tests for the <see cref="BeerUpdatedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerUpdatedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly BeerUpdatedValidator _validator;

    /// <summary>
    ///     Setups BeerUpdatedValidatorTests.
    /// </summary>
    public BeerUpdatedValidatorTests()
    {
        _validator = new BeerUpdatedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public void BeerUpdated_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var beerUpdatedEvent = new BeerUpdated
        {
            Name = new string('x', 15)
        };

        // Act
        var result = _validator.TestValidate(beerUpdatedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name exceeds maximum length.
    /// </summary>
    [Fact]
    public void BeerUpdated_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var beerUpdatedEvent = new BeerUpdated
        {
            Name = new string('x', 201)
        };

        // Act
        var result = _validator.TestValidate(beerUpdatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name is empty.
    /// </summary>
    [Fact]
    public void BeerUpdated_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var beerUpdatedEvent = new BeerUpdated
        {
            Name = string.Empty
        };

        // Act
        var result = _validator.TestValidate(beerUpdatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}