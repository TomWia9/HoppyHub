using Application.Breweries.Commands.UpdateBrewery;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Breweries.Commands.UpdateBrewery;

/// <summary>
///     Unit tests for the <see cref="UpdateBreweryCommandValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateBreweryCommandValidatorTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UpdateBreweryCommandValidator _validator;

    /// <summary>
    ///     Setups UpdateBreweryCommandValidatorTests.
    /// </summary>
    public UpdateBreweryCommandValidatorTests()
    {
        var breweriesDbSetMock = new List<Brewery>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        Mock<IDateTime> dateTimeMock = new();
        dateTimeMock.Setup(x => x.Now).Returns(new DateTime(2023, 3, 29));
        _validator = new UpdateBreweryCommandValidator(_contextMock.Object, dateTimeMock.Object);
    }
    
    /// <summary>
    ///     Tests that validation should not have error for Description when Description is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForDescription_WhenDescriptionIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
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
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Description = new string('x', 5001)
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
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionIsEmpty()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Description = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should not have error for FoundationYear when FoundationYear is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForFoundationYear_WhenFoundationYearIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand
        {
            FoundationYear = 1990
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FoundationYear);
    }

    /// <summary>
    ///     Tests that validation should have error for FoundationYear when FoundationYear is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9999)]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForFoundationYear_WhenFoundationYearIsOutOfRange(
        int foundationYear)
    {
        // Arrange
        var command = new UpdateBreweryCommand
        {
            FoundationYear = foundationYear
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FoundationYear);
    }

    /// <summary>
    ///     Tests that validation should not have error for Street when Street is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForStreet_WhenStreetIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Street = "Test Street"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Street);
    }

    /// <summary>
    ///     Tests that validation should have error for Street when Street exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForStreet_WhenStreetExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Street = new string('x', 201)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Street);
    }

    /// <summary>
    ///     Tests that validation should have error for Street when Street is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForStreet_WhenStreetIsEmpty()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Street = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Street);
    }

    /// <summary>
    ///     Tests that validation should not have error for Number when Number is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForNumber_WhenNumberIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Number = "2D"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
    }

    /// <summary>
    ///     Tests that validation should have error for Number when Number exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForNumber_WhenNumberExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Number = new string('x', 11)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Number);
    }

    /// <summary>
    ///     Tests that validation should have error for Number when Number is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForNumber_WhenNumberIsEmpty()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Number = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Number);
    }

    /// <summary>
    ///     Tests that validation should not have error for PostCode when PostCode is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForPostCode_WhenPostCodeIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            PostCode = "12-345"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PostCode);
    }

    /// <summary>
    ///     Tests that validation should have error for PostCode when PostCode is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForPostCode_WhenPostCodeIsEmpty()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            PostCode = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PostCode);
    }

    /// <summary>
    ///     Tests that validation should have error for PostCode when PostCode has invalid format.
    /// </summary>
    [Theory]
    [InlineData("1")]
    [InlineData("123456")]
    [InlineData("123-56")]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForPostCode_WhenPostCodeHasInvalidFormat(
        string postCode)
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            PostCode = postCode
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PostCode);
    }

    /// <summary>
    ///     Tests that validation should not have error for City when City is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForCity_WhenCityIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            City = "Test City"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    ///     Tests that validation should have error for City when City exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForCity_WhenCityExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            City = new string('x', 51)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    ///     Tests that validation should have error for City when City is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForCity_WhenCityIsEmpty()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            City = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    ///     Tests that validation should not have error for State when State is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForState_WhenStateIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            State = "Test City"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.State);
    }

    /// <summary>
    ///     Tests that validation should have error for State when State exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForState_WhenStateExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            State = new string('x', 51)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    /// <summary>
    ///     Tests that validation should have error for State when State is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForState_WhenStateIsEmpty()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            State = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    /// <summary>
    ///     Tests that validation should not have error for Country when Country is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForCountry_WhenCountryIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Country = "Test Country"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should have error for Country when Country exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForCountry_WhenCountryExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Country = new string('x', 51)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should have error for Country when Country is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForCountry_WhenCountryIsEmpty()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Country = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBreweryCommand_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Name = "Test Name"
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
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
        {
            Name = new string('x', 501)
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
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var command = new UpdateBreweryCommand()
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
    public async Task UpdateBreweryCommand_ShouldHaveValidationErrorForName_WhenNameIsNotUnique()
    {
        // Arrange
        var command = new UpdateBreweryCommand
        {
            Id = Guid.NewGuid(),
            Name = "Test Beer"
        };

        var breweries = new List<Brewery>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Beer"
            }
        };

        var beerDbSetMock = breweries.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries).Returns(beerDbSetMock.Object);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("The brewery name must be unique.");
    }
}