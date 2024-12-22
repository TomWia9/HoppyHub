using Application.Beers.Commands.Common;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Beers.Commands.Common;

/// <summary>
///     Unit tests for the <see cref="BaseBeerCommandValidator{TCommand}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BaseBeerCommandValidatorTests
{
    /// <summary>
    ///     The TestBaseBeerCommand validator instance.
    /// </summary>
    private readonly TestBaseBeerCommandValidator _validator;

    /// <summary>
    ///     Setups BaseBeerCommandValidatorTests.
    /// </summary>
    public BaseBeerCommandValidatorTests()
    {
        _validator = new TestBaseBeerCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for BreweryId when BreweryId is valid.
    /// </summary>
    [Fact]
    public void BaseBeerCommand_ShouldNotHaveValidationErrorForBreweryId_WhenBreweryIdIsValid()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            BreweryId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BreweryId);
    }

    /// <summary>
    ///     Tests that validation should have error for BreweryId when BreweryId is empty.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForBreweryId_WhenBreweryIdIsEmpty()
    {
        var command = new TestBaseBeerCommand
        {
            BreweryId = null
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BreweryId);
    }

    /// <summary>
    ///     Tests that validation should not have error for AlcoholByVolume when AlcoholByVolume is valid.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldNotHaveValidationErrorForAlcoholByVolume_WhenAlcoholByVolumeIsValid()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            AlcoholByVolume = 5.5
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should have error for AlcoholByVolume when AlcoholByVolume is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForAlcoholByVolume_WhenAlcoholByVolumeIsOutOfRange(
        double alcoholByVolume)
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            AlcoholByVolume = alcoholByVolume
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should not have error for Description when Description is valid.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldNotHaveValidationErrorForDescription_WhenDescriptionIsValid()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Description = "Test description"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should have error for Description when Description exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Description = new string('x', 3001)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should not have error for Composition when Composition is valid.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldNotHaveValidationErrorForComposition_WhenCompositionIsValid()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Composition = "Test composition"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Composition);
    }

    /// <summary>
    ///     Tests that validation should have error for Composition when Composition exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForComposition_WhenCompositionExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Composition = new string('x', 301)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Composition);
    }

    /// <summary>
    ///     Tests that validation should not have error for Blg when Blg is valid.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldNotHaveValidationErrorForBlg_WhenBlgIsValid()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Blg = 20
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Blg);
    }

    /// <summary>
    ///     Tests that validation should have error for Blg when Blg is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForBlg_WhenBlgIsOutOfRange(double blg)
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Blg = blg
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Blg);
    }

    /// <summary>
    ///     Tests that validation should not have error for BeerStyleId when BeerStyleId is valid.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldNotHaveValidationErrorForBeerStyleId_WhenBeerStyleIdIsValid()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            BeerStyleId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BeerStyleId);
    }

    /// <summary>
    ///     Tests that validation should have error for BeerStyleId when BeerStyleId is empty.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForBeerStyleId_WhenBeerStyleIdIsEmpty()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            BeerStyleId = null
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BeerStyleId);
    }

    /// <summary>
    ///     Tests that validation should not have error for Ibu when Ibu is valid.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldNotHaveValidationErrorForIbu_WhenIbuIsValid()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Ibu = 40
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Ibu);
    }

    /// <summary>
    ///     Tests that validation should have error for Ibu when Ibu is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(201)]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForIbu_WhenIbuIsOutOfRange(int ibu)
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Ibu = ibu
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ibu);
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            BreweryId = Guid.NewGuid(),
            Name = "Test Beer"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name is empty.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Name = ""
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task BaseBeerCommand_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBeerCommand
        {
            Name = new string('x', 201)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     The TestBaseBeer command.
    /// </summary>
    private record TestBaseBeerCommand : BaseBeerCommand;

    /// <summary>
    ///     The TestBaseBeerCommand validator.
    /// </summary>
    private class TestBaseBeerCommandValidator : BaseBeerCommandValidator<TestBaseBeerCommand>
    {
    }
}