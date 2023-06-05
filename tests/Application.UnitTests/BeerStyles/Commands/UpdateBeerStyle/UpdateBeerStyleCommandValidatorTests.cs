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
}