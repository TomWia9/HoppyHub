using FluentValidation.TestHelper;
using SharedEvents.Responses;
using SharedUtilities.EventValidators.Images;

namespace SharedUtilities.UnitTests.EventValidators.Images;

/// <summary>
///     Unit tests for the <see cref="ImageUploadedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImageUploadedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly ImageUploadedValidator _validator;

    /// <summary>
    ///     Setups ImageUploadedValidatorTests.
    /// </summary>
    public ImageUploadedValidatorTests()
    {
        _validator = new ImageUploadedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Uri when Uri is valid.
    /// </summary>
    [Fact]
    public void ImageUploaded_ShouldNotHaveValidationErrorForUri_WhenUriIsValid()
    {
        // Arrange
        var imageUploadedEvent = new ImageUploaded
        {
            Uri = "https://test.com/test.jpg"
        };

        // Act
        var result = _validator.TestValidate(imageUploadedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Uri);
    }

    /// <summary>
    ///     Tests that validation should have error for Uri when Uri is null or empty.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ImageUploaded_ShouldHaveValidationErrorForUri_WhenUriIsNullOrEmpty(string uri)
    {
        // Arrange
        var imageUploadedEvent = new ImageUploaded
        {
            Uri = uri
        };

        // Act
        var result = _validator.TestValidate(imageUploadedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Uri);
    }

    /// <summary>
    ///     Tests that validation should have error for Uri when Uri is invalid.
    /// </summary>
    [Fact]
    public void ImageUploaded_ShouldHaveValidationErrorForUri_WhenUriIsInvalid()
    {
        // Arrange
        var imageUploadedEvent = new ImageUploaded
        {
            Uri = new string('x', 5)
        };

        // Act
        var result = _validator.TestValidate(imageUploadedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Uri);
    }
}