using Application.BeerStyles.Commands.Common;
using FluentValidation.TestHelper;

namespace Application.UnitTests.BeerStyles.Commands.Common;

/// <summary>
///     Unit tests for the <see cref="BaseBeerStyleCommandValidator{TCommand}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BaseBeerStyleCommandValidatorTests
{
    /// <summary>
    ///     The TestBaseBeerStyle command.
    /// </summary>
    private record TestBaseBeerStyleCommand : BaseBeerStyleCommand;

    /// <summary>
    ///     The TestBaseBeerStyleCommand validator.
    /// </summary>
    private class TestBaseBeerStyleCommandValidator : BaseBeerStyleCommandValidator<TestBaseBeerStyleCommand>
    {
    }

    /// <summary>
    ///     The TestBaseBeerStyleCommand validator instance.
    /// </summary>
    private readonly TestBaseBeerStyleCommandValidator _validator;

    /// <summary>
    ///     Setups BaseBeerStyleCommandValidatorTests.
    /// </summary>
    public BaseBeerStyleCommandValidatorTests()
    {
        _validator = new TestBaseBeerStyleCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public void BaseBeerStyleCommand_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand()
        {
            Name = "India Pale Ale"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseBeerStyleCommand_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand()
        {
            Name = new string('x', 101)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name is empty.
    /// </summary>
    [Fact]
    public void BaseBeerStyleCommand_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand()
        {
            Name = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should not have error for Description when Description is valid.
    /// </summary>
    [Fact]
    public void BaseBeerStyleCommand_ShouldNotHaveValidationErrorForDescription_WhenDescriptionIsValid()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand
        {
            Description = "Test description"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should have error for Description when Description exceeds maximum length.
    /// </summary>
    [Fact]
    public void
        BaseBeerStyleCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand()
        {
            Description = new string('x', 1001)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should have error for Description when Description is empty.
    /// </summary>
    [Fact]
    public void BaseBeerStyleCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionIsEmpty()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand()
        {
            Description = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should not have error for CountryOfOrigin when CountryOfOrigin is valid.
    /// </summary>
    [Fact]
    public void BaseBeerStyleCommand_ShouldNotHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginIsValid()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand
        {
            CountryOfOrigin = "Poland"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CountryOfOrigin);
    }

    /// <summary>
    ///     Tests that validation should have error for CountryOfOrigin when CountryOfOrigin exceeds maximum length.
    /// </summary>
    [Fact]
    public void
        BaseBeerStyleCommand_ShouldHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand()
        {
            CountryOfOrigin = new string('x', 51)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CountryOfOrigin);
    }

    /// <summary>
    ///     Tests that validation should have error for CountryOfOrigin when CountryOfOrigin is empty.
    /// </summary>
    [Fact]
    public void BaseBeerStyleCommand_ShouldHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginIsEmpty()
    {
        // Arrange
        var command = new TestBaseBeerStyleCommand()
        {
            CountryOfOrigin = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CountryOfOrigin);
    }
}