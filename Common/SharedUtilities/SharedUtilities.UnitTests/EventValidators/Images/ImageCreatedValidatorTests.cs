using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Images;

namespace SharedUtilities.UnitTests.EventValidators.Images;

/// <summary>
///     Unit tests for the <see cref="ImageCreatedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImageCreatedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly ImageCreatedValidator _validator;

    /// <summary>
    ///     Setups ImageCreatedValidatorTests.
    /// </summary>
    public ImageCreatedValidatorTests()
    {
        _validator = new ImageCreatedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Path when Path is valid.
    /// </summary>
    [Fact]
    public void ImageCreated_ShouldNotHaveValidationErrorForPath_WhenPathIsValid()
    {
        // Arrange
        var imageCreatedEvent = new ImageCreated
        {
            Path = new string('x', 15)
        };

        // Act
        var result = _validator.TestValidate(imageCreatedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Path);
    }

    /// <summary>
    ///     Tests that validation should have error for Path when Path exceeds maximum length.
    /// </summary>
    [Fact]
    public void ImageCreated_ShouldHaveValidationErrorForPath_WhenPathExceedsMaximumLength()
    {
        // Arrange
        var imageCreatedEvent = new ImageCreated
        {
            Path = new string('x', 257)
        };

        // Act
        var result = _validator.TestValidate(imageCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Path);
    }

    /// <summary>
    ///     Tests that validation should have error for Path when Path is null or empty.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ImageCreated_ShouldHaveValidationErrorForPath_WhenPathIsNullOrEmpty(string path)
    {
        // Arrange
        var imageCreatedEvent = new ImageCreated
        {
            Path = path
        };

        // Act
        var result = _validator.TestValidate(imageCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Path);
    }

    /// <summary>
    ///     Tests that validation should not have error for Image when Image is valid.
    /// </summary>
    [Fact]
    public void ImageCreated_ShouldNotHaveValidationErrorForImage_WhenImageIsValid()
    {
        // Arrange
        const int imageSize = 2 * 1024 * 1024; //2MB
        var imageCreatedEvent = new ImageCreated
        {
            Image = new byte[imageSize]
        };

        // Act
        var result = _validator.TestValidate(imageCreatedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Image);
    }

    /// <summary>
    ///     Tests that validation should have error for Image when Image is null or empty.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData(new byte[0])]
    public void ImageCreated_ShouldHaveValidationErrorForImage_WhenImageIsNullOrEmpty(byte[] image)
    {
        // Arrange
        var imageCreatedEvent = new ImageCreated
        {
            Image = image
        };

        // Act
        var result = _validator.TestValidate(imageCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Image);
    }

    /// <summary>
    ///     Tests that validation should have error for Image when Image exceeds maximum size.
    /// </summary>
    [Fact]
    public void ImageCreated_ShouldHaveValidationErrorForImage_WhenImageExceedsMaximumSize()
    {
        // Arrange
        const int imageSize = 6 * 1024 * 1024; //6MB

        var imageCreatedEvent = new ImageCreated
        {
            Image = new byte[imageSize]
        };

        // Act
        var result = _validator.TestValidate(imageCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Image).WithErrorMessage("The file exceeds the maximum size of 5MB.");
    }
}