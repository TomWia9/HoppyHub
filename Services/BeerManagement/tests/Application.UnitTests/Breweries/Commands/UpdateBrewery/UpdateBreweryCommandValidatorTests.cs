using Application.Breweries.Commands.UpdateBrewery;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Breweries.Commands.UpdateBrewery;

/// <summary>
///     Unit tests for the <see cref="UpdateBreweryCommandValidator" /> class.
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
        var breweriesDbSetMock = Enumerable.Empty<Brewery>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        Mock<TimeProvider> timeProviderMock = new();
        timeProviderMock.Setup(x => x.GetUtcNow()).Returns(new DateTime(2023, 3, 29));
        _validator = new UpdateBreweryCommandValidator(_contextMock.Object, timeProviderMock.Object);
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