using Application.BeerImages.Commands.UpsertBeerImage;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.UnitTests.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     Unit tests for the <see cref="UpsertBeerImageCommandValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpsertBeerImageCommandValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UpsertBeerImageCommandValidator _validator;

    /// <summary>
    ///     Setups UpsertBeerImageCommandValidatorTests.
    /// </summary>
    public UpsertBeerImageCommandValidatorTests()
    {
        _validator = new UpsertBeerImageCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for BeerId when BeerId is valid.
    /// </summary>
    [Fact]
    public void UpsertBeerImageCommand_ShouldNotHaveValidationErrorForBeerId_WhenBeerIdIsValid()
    {
        // Arrange
        var command = new UpsertBeerImageCommand
        {
            BeerId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BeerId);
    }

    /// <summary>
    ///     Tests that validation should have error for BeerId when BeerId is empty.
    /// </summary>
    [Fact]
    public void UpsertBeerImageCommand_ShouldHaveValidationErrorForBeerId_WhenBeerIdIsEmpty()
    {
        // Arrange
        var command = new UpsertBeerImageCommand
        {
            BeerId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BeerId);
    }

    /// <summary>
    ///     Tests that validation should not have error for Image when Image is valid.
    /// </summary>
    [Fact]
    public void UpsertBeerImageCommand_ShouldNotHaveValidationErrorForImage_WhenImageIsValid()
    {
        // Arrange
        const int imageSize = 2 * 1024 * 1024; //2MB
        const string imageFileName = "test.jpg";

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(x => x.Length).Returns(imageSize);
        imageMock.Setup(x => x.FileName).Returns(imageFileName);

        var command = new UpsertBeerImageCommand
        {
            Image = imageMock.Object
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Image);
    }

    /// <summary>
    ///     Tests that validation should have error for Image when Image is null.
    /// </summary>
    [Fact]
    public void UpsertBeerImageCommand_ShouldHaveValidationErrorForImage_WhenImageIsNull()
    {
        // Arrange
        var command = new UpsertBeerImageCommand
        {
            Image = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Image);
    }

    /// <summary>
    ///     Tests that validation should have error for Image when Image has invalid extension.
    /// </summary>
    [Fact]
    public void UpsertBeerImageCommand_ShouldHaveValidationErrorForImage_WhenImageHasInvalidExtension()
    {
        // Arrange
        const int imageSize = 2 * 1024 * 1024; //2MB
        const string imageFileName = "test.pdf";

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(x => x.Length).Returns(imageSize);
        imageMock.Setup(x => x.FileName).Returns(imageFileName);

        var command = new UpsertBeerImageCommand
        {
            Image = imageMock.Object
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Image).WithErrorMessage("Only JPG and PNG files are allowed.");
    }

    /// <summary>
    ///     Tests that validation should have error for Image when Image exceeds maximum size.
    /// </summary>
    [Fact]
    public void UpsertBeerImageCommand_ShouldHaveValidationErrorForImage_WhenImageExceedsMaximumSize()
    {
        // Arrange
        const int imageSize = 6 * 1024 * 1024; //6MB
        const string imageFileName = "test.png";

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(x => x.Length).Returns(imageSize);
        imageMock.Setup(x => x.FileName).Returns(imageFileName);

        var command = new UpsertBeerImageCommand
        {
            Image = imageMock.Object
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Image).WithErrorMessage("The file exceeds the maximum size of 5MB.");
    }
}