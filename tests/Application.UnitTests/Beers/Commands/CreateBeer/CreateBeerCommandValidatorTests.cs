using Application.Beers.Commands.CreateBeer;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Beers.Commands.CreateBeer;

/// <summary>
///     Unit tests for the <see cref="CreateBeerCommandValidator" /> class.
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