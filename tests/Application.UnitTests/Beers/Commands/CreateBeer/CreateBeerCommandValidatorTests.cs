using Application.Beers.Commands.CreateBeer;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Beers.Commands.CreateBeer;

/// <summary>
///     Unit tests for the <see cref="CreateBeerCommandValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBeerCommandValidatorTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly CreateBeerCommandValidator _validator;

    /// <summary>
    ///     Setups CreateBeerCommandValidatorTests.
    /// </summary>
    public CreateBeerCommandValidatorTests()
    {
        var beerDbSetMock = new List<Beer>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(x => x.Beers).Returns(beerDbSetMock.Object);
        _validator = new CreateBeerCommandValidator(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for BreweryId when BreweryId is valid.
    /// </summary>
    [Fact]
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForBreweryId_WhenBreweryIdIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
        {
            BreweryId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BreweryId);
    }

    /// <summary>
    ///     Tests that validation should have error for BreweryId when BreweryId is empty.
    /// </summary>
    [Fact]
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForBreweryId_WhenBreweryIdIsEmpty()
    {
        // Arrange
        var command = new CreateBeerCommand
        {
            BreweryId = Guid.Empty
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
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForAlcoholByVolume_WhenAlcoholByVolumeIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForAlcoholByVolume_WhenAlcoholByVolumeIsOutOfRange(
        double alcoholByVolume)
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForDescription_WhenDescriptionIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForDescription_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForComposition_WhenCompositionIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForComposition_WhenCompositionExceedsMaximumLength()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForBlg_WhenBlgIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForBlg_WhenBlgIsOutOfRange(double blg)
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForPlato_WhenPlatoIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForPlato_WhenPlatoIsOutOfRange(double plato)
    {
        // Arrange
        var command = new CreateBeerCommand
        {
            Plato = plato
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Plato);
    }

    /// <summary>
    ///     Tests that validation should not have error for BeerStyleId when BeerStyleId is valid.
    /// </summary>
    [Fact]
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForBeerStyleId_WhenBeerStyleIdIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForBeerStyleId_WhenBeerStyleIdIsEmpty()
    {
        // Arrange
        var command = new CreateBeerCommand
        {
            BeerStyleId = Guid.Empty
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
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForIbu_WhenIbuIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForIbu_WhenIbuIsOutOfRange(int ibu)
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldNotHaveValidationErrorForName_WhenNameIsValid()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForName_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForName_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new CreateBeerCommand
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
    public async Task CreateBeerCommand_ShouldHaveValidationErrorForName_WhenNameIsNotUniqueWithinBrewery()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var command = new CreateBeerCommand
        {
            BreweryId = breweryId,
            Name = "Test Beer"
        };

        var beers = new List<Beer>
        {
            new()
            {
                BreweryId = breweryId,
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