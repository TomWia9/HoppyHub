using FluentValidation.TestHelper;
using SharedEvents;
using SharedUtilities.EventValidators.Images;

namespace SharedUtilities.UnitTests.EventValidators.Images;

/// <summary>
///     Unit tests for the <see cref="ImagesDeletedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImagesDeletedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly ImagesDeletedValidator _validator;

    /// <summary>
    ///     Setups ImagesDeletedValidatorTests.
    /// </summary>
    public ImagesDeletedValidatorTests()
    {
        _validator = new ImagesDeletedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Paths when Paths are valid.
    /// </summary>
    [Fact]
    public void ImagesDeleted_ShouldNotHaveValidationErrorForPaths_WhenPathsAreValid()
    {
        // Arrange
        var imagesDeletedEvent = new ImagesDeleted
        {
            Paths = new List<string>
            {
                new('x', 15),
                new('y', 20)
            }
        };

        // Act
        var result = _validator.TestValidate(imagesDeletedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Paths);
    }

    /// <summary>
    ///     Tests that validation should have error for Paths when at least one of Paths exceeds maximum length.
    /// </summary>
    [Fact]
    public void ImagesDeleted_ShouldHaveValidationErrorForPaths_WhenAtLeastOneOfPathsExceedsMaximumLength()
    {
        // Arrange
        var imagesDeletedEvent = new ImagesDeleted
        {
            Paths = new List<string>
            {
                new('x', 15),
                new('y', 257)
            }
        };

        // Act
        var result = _validator.TestValidate(imagesDeletedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Paths);
    }

    /// <summary>
    ///     Tests that validation should have error for Paths when at least one of Paths is empty.
    /// </summary>
    [Fact]
    public void ImagesDeleted_ShouldHaveValidationErrorForPaths_WhenAtLeastOneOfPathsIsEmpty()
    {
        // Arrange
        var imagesDeletedEvent = new ImagesDeleted
        {
            Paths = new List<string>
            {
                new('x', 15),
                string.Empty
            }
        };

        // Act
        var result = _validator.TestValidate(imagesDeletedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Paths);
    }

    /// <summary>
    ///     Tests that validation should have error for Paths when Paths are null.
    /// </summary>
    [Fact]
    public void ImagesDeleted_ShouldHaveValidationErrorForPaths_WhenPathsAreNull()
    {
        // Arrange
        var imagesDeletedEvent = new ImagesDeleted
        {
            Paths = null
        };

        // Act
        var result = _validator.TestValidate(imagesDeletedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Paths);
    }

    /// <summary>
    ///     Tests that validation should have error for Paths when Paths are empty.
    /// </summary>
    [Fact]
    public void ImagesDeleted_ShouldHaveValidationErrorForPaths_WhenPathsAreEmpty()
    {
        // Arrange
        var imagesDeletedEvent = new ImagesDeleted
        {
            Paths = Enumerable.Empty<string>()
        };

        // Act
        var result = _validator.TestValidate(imagesDeletedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Paths);
    }
}