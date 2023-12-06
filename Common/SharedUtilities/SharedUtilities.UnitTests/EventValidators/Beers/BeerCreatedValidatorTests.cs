using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Beers;

namespace SharedUtilities.UnitTests.EventValidators.Beers;

/// <summary>
///     Unit tests for the <see cref="BeerCreatedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerCreatedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly BeerCreatedValidator _validator;

    /// <summary>
    ///     Setups BeerCreatedValidatorTests.
    /// </summary>
    public BeerCreatedValidatorTests()
    {
        _validator = new BeerCreatedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public void BeerCreated_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var beerCreatedEvent = new BeerCreated
        {
            Name = new string('x', 15)
        };

        // Act
        var result = _validator.TestValidate(beerCreatedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name exceeds maximum length.
    /// </summary>
    [Fact]
    public void BeerCreated_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var beerCreatedEvent = new BeerCreated
        {
            Name = new string('x', 201)
        };

        // Act
        var result = _validator.TestValidate(beerCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name is empty.
    /// </summary>
    [Fact]
    public void BeerCreated_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var beerCreatedEvent = new BeerCreated
        {
            Name = string.Empty
        };

        // Act
        var result = _validator.TestValidate(beerCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should not have error for BreweryName when BreweryName is valid.
    /// </summary>
    [Fact]
    public void BeerCreated_ShouldNotHaveValidationErrorForBreweryName_WhenBreweryNameIsValid()
    {
        // Arrange
        var beerCreatedEvent = new BeerCreated
        {
            BreweryName = new string('x', 250)
        };

        // Act
        var result = _validator.TestValidate(beerCreatedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BreweryName);
    }

    /// <summary>
    ///     Tests that validation should have error for BreweryName when BreweryName exceeds maximum length.
    /// </summary>
    [Fact]
    public void BeerCreated_ShouldHaveValidationErrorForBreweryName_WhenBreweryNameExceedsMaximumLength()
    {
        // Arrange
        var beerCreatedEvent = new BeerCreated
        {
            BreweryName = new string('x', 501)
        };

        // Act
        var result = _validator.TestValidate(beerCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BreweryName);
    }

    /// <summary>
    ///     Tests that validation should have error for BreweryName when BreweryName is empty.
    /// </summary>
    [Fact]
    public void BeerCreated_ShouldHaveValidationErrorForBreweryName_WhenBreweryNameIsEmpty()
    {
        // Arrange
        var beerCreatedEvent = new BeerCreated
        {
            BreweryName = string.Empty
        };

        // Act
        var result = _validator.TestValidate(beerCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BreweryName);
    }
}