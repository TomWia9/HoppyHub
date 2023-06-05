using Application.Common.Interfaces;
using Application.Favorites.Commands.CreateFavorite;
using Domain.Entities;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Favorites.Commands.CreateFavorite;

/// <summary>
///     Unit tests for the <see cref="CreateFavoriteCommandValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateFavoriteCommandValidatorTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly CreateFavoriteCommandValidator _validator;

    /// <summary>
    ///     Initializes CreateFavoriteCommandValidatorTests.
    /// </summary>
    public CreateFavoriteCommandValidatorTests()
    {
        var favoritesDbSetMock = Enumerable.Empty<Favorite>().AsQueryable().BuildMockDbSet();
        _contextMock = new Mock<IApplicationDbContext>();
        Mock<ICurrentUserService> currentUserServiceMock = new();
        _contextMock.Setup(x => x.Favorites).Returns(favoritesDbSetMock.Object);
        _validator = new CreateFavoriteCommandValidator(_contextMock.Object, currentUserServiceMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for BeerId when BeerId is valid.
    /// </summary>
    [Fact]
    public async Task CreateFavoriteCommand_ShouldNotHaveValidationErrorForBeerId_WhenBeerIdIsValid()
    {
        // Arrange
        var command = new CreateFavoriteCommand
        {
            BeerId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BeerId);
    }

    /// <summary>
    ///     Tests that validation should have error for BeerId when user is adding same beer to favorites again.
    /// </summary>
    [Fact]
    public async Task
        CreateFavoriteCommand_ShouldHaveValidationErrorForBeerId_WhenUserIsAddingSameBeerToFavoritesAgain()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var command = new CreateFavoriteCommand
        {
            BeerId = beerId
        };
        var favorites = new List<Favorite>
        {
            new()
            {
                BeerId = beerId,
            }
        };
        var favoritesDbSetMock = favorites.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Favorites).Returns(favoritesDbSetMock.Object);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BeerId).WithErrorMessage("Only one favorite per beer is allowed.");
    }
}