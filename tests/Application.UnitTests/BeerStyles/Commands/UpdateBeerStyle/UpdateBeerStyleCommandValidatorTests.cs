using Application.BeerStyles.Commands.UpdateBeerStyle;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.BeerStyles.Commands.UpdateBeerStyle;

/// <summary>
///     Unit tests for the <see cref="UpdateBeerStyleCommandValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateBeerStyleCommandValidatorTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UpdateBeerStyleCommandValidator _validator;

    /// <summary>
    ///     Setups UpdateBeerStyleCommandValidatorTests.
    /// </summary>
    public UpdateBeerStyleCommandValidatorTests()
    {
        var beerStylesDbSetMock = Enumerable.Empty<BeerStyle>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _validator = new UpdateBeerStyleCommandValidator(_contextMock.Object);
    }
    
     /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyleCommand_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand()
        {
            Name = "India Pale Ale"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyleCommand_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand()
        {
            Name = new string('x', 101)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyleCommand_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand()
        {
            Name = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name is not unique.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyleCommand_ShouldHaveValidationErrorForName_WhenNameIsNotUnique()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand
        {
            Id = Guid.NewGuid(),
            Name = "Pils"
        };

        var beerStyles = new List<BeerStyle>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Pils"
            }
        };

        var beerDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.BeerStyles).Returns(beerDbSetMock.Object);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("The beer style name must be unique.");
    }

    /// <summary>
    ///     Tests that validation should not have error for Description when Description is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyleCommand_ShouldNotHaveValidationErrorForDescription_WhenDescriptionIsValid()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand
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
    public async Task
        UpdateBeerStyleCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand()
        {
            Description = new string('x', 1001)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should have error for Description when Description is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyleCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionIsEmpty()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand()
        {
            Description = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should not have error for CountryOfOrigin when CountryOfOrigin is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyleCommand_ShouldNotHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginIsValid()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand
        {
            CountryOfOrigin = "Poland"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CountryOfOrigin);
    }

    /// <summary>
    ///     Tests that validation should have error for CountryOfOrigin when CountryOfOrigin exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task
        UpdateBeerStyleCommand_ShouldHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand()
        {
            CountryOfOrigin = new string('x', 51)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CountryOfOrigin);
    }

    /// <summary>
    ///     Tests that validation should have error for CountryOfOrigin when CountryOfOrigin is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyleCommand_ShouldHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginIsEmpty()
    {
        // Arrange
        var command = new UpdateBeerStyleCommand()
        {
            CountryOfOrigin = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CountryOfOrigin);
    }
}