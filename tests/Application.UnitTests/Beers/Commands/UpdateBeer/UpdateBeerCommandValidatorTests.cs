using Application.Beers.Commands.UpdateBeer;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Beers.Commands.UpdateBeer;

/// <summary>
///     Unit tests for the <see cref="UpdateBeerCommandValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateBeerCommandValidatorTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UpdateBeerCommandValidator _validator;

    /// <summary>
    ///     Setups UpdateBeerCommandValidatorTests.
    /// </summary>
    public UpdateBeerCommandValidatorTests()
    {
        var beerDbSetMock = new List<Beer>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(x => x.Beers).Returns(beerDbSetMock.Object);
        _validator = new UpdateBeerCommandValidator(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for Brewery when Brewery is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenBreweryIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Brewery = "Test Brewery"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Brewery);
    }

    /// <summary>
    ///     Tests that validation should have error for Brewery when Brewery is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenBreweryIsEmpty()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Brewery = ""
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Brewery);
    }

    /// <summary>
    ///     Tests that validation should have error for Brewery when Brewery exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenBreweryExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Brewery = new string('x', 201)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Brewery);
    }

    /// <summary>
    ///     Tests that validation should not have error for AlcoholByVolume when AlcoholByVolume is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenAlcoholByVolumeIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
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
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenAlcoholByVolumeIsOutOfRange(
        double alcoholByVolume)
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            AlcoholByVolume = alcoholByVolume
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AlcoholByVolume);
    }

    /// <summary>
    ///     Tests that validation should not have error for Brewery when Brewery is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenDescriptionIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
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
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Description = new string('x', 3001)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    /// <summary>
    ///     Tests that validation should not have error for Blg when Blg is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenBlgIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
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
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenBlgIsOutOfRange(double blg)
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Blg = blg
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Blg);
    }

    /// <summary>
    ///     Tests that validation should not have error for Plato when Plato is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenPlatoIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Plato = 20
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Plato);
    }

    /// <summary>
    ///     Tests that validation should have error for Plato when Plato is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenPlatoIsOutOfRange(double plato)
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Plato = plato
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Plato);
    }

    /// <summary>
    ///     Tests that validation should not have error for Style when Style is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenStyleIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Style = "Test style"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Style);
    }

    /// <summary>
    ///     Tests that validation should have error for Style when Style is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenStyleIsEmpty()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Style = ""
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Style);
    }

    /// <summary>
    ///     Tests that validation should have error for Style when Style exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenStyleExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Style = new string('x', 51)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Style);
    }

    /// <summary>
    ///     Tests that validation should not have error for Ibu when Ibu is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenIbuIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
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
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenIbuIsOutOfRange(int ibu)
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Ibu = ibu
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ibu);
    }

    /// <summary>
    ///     Tests that validation should not have error for Country when Country is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenCountryIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Country = "Poland"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should have error for Country when Country is empty.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenCountryIsEmpty()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Country = ""
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    /// <summary>
    ///     Tests that validation should have error for Country when Country exceeds maximum length.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenCountryExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Country = new string('x', 51)
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
    public async Task UpdateBeerCommand_ShouldNotHaveValidationError_WhenNameIsValid()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Brewery = "Test Brewery",
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
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new UpdateBeerCommand
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
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new UpdateBeerCommand
        {
            Name = new string('x', 201)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name is not unique within brewery.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldHaveValidationError_WhenNameIsNotUniqueWithinBrewery()
    {
        // Arrange
        var command = new UpdateBeerCommand()
        {
            Id = Guid.NewGuid(),
            Brewery = "Test Brewery",
            Name = "Test Beer"
        };

        var beers = new List<Beer>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Brewery = "Test Brewery",
                Name = "Test Beer"
            }
        };

        var beerDbSetMock = beers.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beerDbSetMock.Object);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("The beer name must be unique within the brewery.");
    }
}