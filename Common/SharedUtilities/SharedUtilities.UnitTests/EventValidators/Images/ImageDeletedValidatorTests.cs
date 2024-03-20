using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Images;

namespace SharedUtilities.UnitTests.EventValidators.Images;

/// <summary>
///     Unit tests for the <see cref="ImageDeletedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImageDeletedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly ImageDeletedValidator _validator;

    /// <summary>
    ///     Setups ImageDeletedValidatorTests.
    /// </summary>
    public ImageDeletedValidatorTests()
    {
        _validator = new ImageDeletedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Uri when Uri is valid.
    /// </summary>
    [Fact]
    public void ImageDeleted_ShouldNotHaveValidationErrorForUri_WhenUriIsValid()
    {
        // Arrange
        var imageDeletedEvent = new ImageDeleted
        {
            Uri = "https://test.com/test.jpg"
        };

        // Act
        var result = _validator.TestValidate(imageDeletedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Uri);
    }

    /// <summary>
    ///     Tests that validation should have error for Uri when Uri is null or empty.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ImageDeleted_ShouldHaveValidationErrorForUri_WhenUriIsNullOrEmpty(string? uri)
    {
        // Arrange
        var imageDeletedEvent = new ImageDeleted
        {
            Uri = uri
        };

        // Act
        var result = _validator.TestValidate(imageDeletedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Uri);
    }

    /// <summary>
    ///     Tests that validation should have error for Uri when Uri is invalid.
    /// </summary>
    [Fact]
    public void ImageDeleted_ShouldHaveValidationErrorForUri_WhenUriIsInvalid()
    {
        // Arrange
        var imageDeletedEvent = new ImageDeleted
        {
            Uri = new string('x', 5)
        };

        // Act
        var result = _validator.TestValidate(imageDeletedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Uri);
    }
}