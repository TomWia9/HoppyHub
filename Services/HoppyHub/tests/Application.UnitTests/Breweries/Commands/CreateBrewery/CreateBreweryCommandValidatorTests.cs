using Application.Breweries.Commands.CreateBrewery;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Breweries.Commands.CreateBrewery;

/// <summary>
///     Unit tests for the <see cref="CreateBreweryCommandValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBreweryCommandValidatorTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly CreateBreweryCommandValidator _validator;

    /// <summary>
    ///     Setups CreateBreweryCommandValidatorTests.
    /// </summary>
    public CreateBreweryCommandValidatorTests()
    {
        var breweriesDbSetMock = Enumerable.Empty<Brewery>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        Mock<IDateTime> dateTimeMock = new();
        dateTimeMock.Setup(x => x.Now).Returns(new DateTime(2023, 3, 29));
        _validator = new CreateBreweryCommandValidator(_contextMock.Object, dateTimeMock.Object);
    }

    /// <summary>
    ///     Tests that validation should have error for Name when Name is not unique.
    /// </summary>
    [Fact]
    public async Task CreateBreweryCommand_ShouldHaveValidationErrorForName_WhenNameIsNotUnique()
    {
        // Arrange
        var command = new CreateBreweryCommand
        {
            Name = "Test Beer"
        };

        var breweries = new List<Brewery>
        {
            new()
            {
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