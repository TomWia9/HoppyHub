using Application.Breweries.Commands.Common;
using Application.Common.Interfaces;
using FluentValidation.TestHelper;
using Moq;

namespace Application.UnitTests.Breweries.Commands.Common;

/// <summary>
///     Unit tests for the <see cref="BaseBreweryCommandValidator{TCommand}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BaseBreweryCommandValidatorTests
{
    /// <summary>
    ///     The TestBaseBrewery command.
    /// </summary>
    private record TestBaseBreweryCommand : BaseBreweryCommand;

    /// <summary>
    ///     The TestBaseBreweryCommand validator.
    /// </summary>
    private class TestBaseBreweryCommandValidator : BaseBreweryCommandValidator<TestBaseBreweryCommand>
    {
        public TestBaseBreweryCommandValidator(IDateTime dateTime) : base(dateTime)
        {
        }
    }

    /// <summary>
    ///     The TestBaseBreweryCommand validator instance.
    /// </summary>
    private readonly TestBaseBreweryCommandValidator _validator;

    /// <summary>
    ///     Setups BaseBreweryCommandValidatorTests.
    /// </summary>
    public BaseBreweryCommandValidatorTests()
    {
        Mock<IDateTime> dateTimeMock = new();
        dateTimeMock.Setup(x => x.Now).Returns(new DateTime(2023, 3, 29));

        _validator = new TestBaseBreweryCommandValidator(dateTimeMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for Description when Description is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForDescription_WhenDescriptionIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand
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
    public void BaseBreweryCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Description = new string('x', 5001)
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
    public void BaseBreweryCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionIsEmpty()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Description = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should not have error for FoundationYear when FoundationYear is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForFoundationYear_WhenFoundationYearIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand
        {
            FoundationYear = 1990
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FoundationYear);
    }

    /// <summary>
    ///     Tests that validation should have error for FoundationYear when FoundationYear is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9999)]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForFoundationYear_WhenFoundationYearIsOutOfRange(
        int foundationYear)
    {
        // Arrange
        var command = new TestBaseBreweryCommand
        {
            FoundationYear = foundationYear
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FoundationYear);
    }

    /// <summary>
    ///     Tests that validation should not have error for WebsiteUrl when WebsiteUrl is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForWebsiteUrl_WhenWebsiteUrlIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand
        {
            WebsiteUrl = "https://www.test.com"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    /// <summary>
    ///     Tests that validation should have error for WebsiteUrl when WebsiteUrl exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForWebsiteUrl_WhenWebsiteUrlExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBreweryCommand
        {
            WebsiteUrl = new string('x', 201)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    /// <summary>
    ///     Tests that validation should have error for WebsiteUrl when WebsiteUrl is invalid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForWebsiteUrl_WhenWebsiteUrlIsInvalid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand
        {
            WebsiteUrl = "test/com"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    /// <summary>
    ///     Tests that validation should not have error for Street when Street is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForStreet_WhenStreetIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Street = "Test Street"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Street);
    }

    /// <summary>
    ///     Tests that validation should have error for Street when Street exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForStreet_WhenStreetExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Street = new string('x', 201)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Street);
    }

    /// <summary>
    ///     Tests that validation should have error for Street when Street is empty.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForStreet_WhenStreetIsEmpty()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Street = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Street);
    }

    /// <summary>
    ///     Tests that validation should not have error for Number when Number is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForNumber_WhenNumberIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Number = "2D"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Number);
    }

    /// <summary>
    ///     Tests that validation should have error for Number when Number exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForNumber_WhenNumberExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Number = new string('x', 11)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Number);
    }

    /// <summary>
    ///     Tests that validation should have error for Number when Number is empty.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForNumber_WhenNumberIsEmpty()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Number = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Number);
    }

    /// <summary>
    ///     Tests that validation should not have error for PostCode when PostCode is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForPostCode_WhenPostCodeIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            PostCode = "12-345"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PostCode);
    }

    /// <summary>
    ///     Tests that validation should have error for PostCode when PostCode is empty.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForPostCode_WhenPostCodeIsEmpty()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            PostCode = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

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
    public void BaseBreweryCommand_ShouldHaveValidationErrorForPostCode_WhenPostCodeHasInvalidFormat(
        string postCode)
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            PostCode = postCode
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PostCode);
    }

    /// <summary>
    ///     Tests that validation should not have error for City when City is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForCity_WhenCityIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            City = "Test City"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    ///     Tests that validation should have error for City when City exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForCity_WhenCityExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            City = new string('x', 51)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    ///     Tests that validation should have error for City when City is empty.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForCity_WhenCityIsEmpty()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            City = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    ///     Tests that validation should not have error for State when State is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForState_WhenStateIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            State = "Test City"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.State);
    }

    /// <summary>
    ///     Tests that validation should have error for State when State exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForState_WhenStateExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            State = new string('x', 51)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    /// <summary>
    ///     Tests that validation should have error for State when State is empty.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForState_WhenStateIsEmpty()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            State = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    /// <summary>
    ///     Tests that validation should not have error for Country when Country is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForCountry_WhenCountryIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Country = "Test Country"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should have error for Country when Country exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForCountry_WhenCountryExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Country = new string('x', 51)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should have error for Country when Country is empty.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldHaveValidationErrorForCountry_WhenCountryIsEmpty()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Country = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should not have error for Name when Name is valid.
    /// </summary>
    [Fact]
    public void BaseBreweryCommand_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Name = "Test Name"
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
    public void BaseBreweryCommand_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Name = new string('x', 501)
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
    public void BaseBreweryCommand_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var command = new TestBaseBreweryCommand()
        {
            Name = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}