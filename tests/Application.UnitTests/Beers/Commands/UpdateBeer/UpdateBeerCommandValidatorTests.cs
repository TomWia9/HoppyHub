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
    ///     Tests that validation should have error for Name when Name is not unique within brewery.
    /// </summary>
    [Fact]
    public async Task UpdateBeerCommand_ShouldHaveValidationErrorForName_WhenNameIsNotUniqueWithinBrewery()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var command = new UpdateBeerCommand()
        {
            Id = Guid.NewGuid(),
            BreweryId = breweryId,
            Name = "Test Beer"
        };

        var beers = new List<Beer>
        {
            new()
            {
                Id = Guid.NewGuid(),
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