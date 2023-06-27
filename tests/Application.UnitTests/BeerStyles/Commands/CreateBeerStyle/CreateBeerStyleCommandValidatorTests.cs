using Application.BeerStyles.Commands.CreateBeerStyle;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.BeerStyles.Commands.CreateBeerStyle;

/// <summary>
///     Unit tests for the <see cref="CreateBeerStyleCommandValidator" /> class.
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
}