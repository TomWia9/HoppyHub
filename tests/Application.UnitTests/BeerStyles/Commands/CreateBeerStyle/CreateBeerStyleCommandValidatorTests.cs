using Application.BeerStyles.Commands.CreateBeerStyle;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.BeerStyles.Commands.CreateBeerStyle;

/// <summary>
///     Unit tests for the <see cref="CreateBeerStyleCommandValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBeerStyleCommandValidatorTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly CreateBeerStyleCommandValidator _validator;

    /// <summary>
    ///     Setups CreateBeerStyleCommandValidatorTests.
    /// </summary>
    public CreateBeerStyleCommandValidatorTests()
    {
        var beerStylesDbSetMock = Enumerable.Empty<BeerStyle>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);
        _validator = new CreateBeerStyleCommandValidator(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public async Task CreateBeerStyleCommand_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var command = new CreateBeerStyleCommand()
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
    public async Task CreateBeerStyleCommand_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new CreateBeerStyleCommand()
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
    public async Task CreateBeerStyleCommand_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateBeerStyleCommand()
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
    public async Task CreateBeerStyleCommand_ShouldHaveValidationErrorForName_WhenNameIsNotUnique()
    {
        // Arrange
        var command = new CreateBeerStyleCommand
        {
            Name = "Pils"
        };

        var beerStyles = new List<BeerStyle>
        {
            new()
            {
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
    public async Task CreateBeerStyleCommand_ShouldNotHaveValidationErrorForDescription_WhenDescriptionIsValid()
    {
        // Arrange
        var command = new CreateBeerStyleCommand
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
        CreateBeerStyleCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = new CreateBeerStyleCommand()
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
    public async Task CreateBeerStyleCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionIsEmpty()
    {
        // Arrange
        var command = new CreateBeerStyleCommand()
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
    public async Task CreateBeerStyleCommand_ShouldNotHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginIsValid()
    {
        // Arrange
        var command = new CreateBeerStyleCommand
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
        CreateBeerStyleCommand_ShouldHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginExceedsMaximumLength()
    {
        // Arrange
        var command = new CreateBeerStyleCommand()
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
    public async Task CreateBeerStyleCommand_ShouldHaveValidationErrorForCountryOfOrigin_WhenCountryOfOriginIsEmpty()
    {
        // Arrange
        var command = new CreateBeerStyleCommand()
        {
            CountryOfOrigin = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CountryOfOrigin);
    }
}