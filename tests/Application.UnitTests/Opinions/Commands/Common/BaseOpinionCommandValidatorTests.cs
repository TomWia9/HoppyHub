using Application.Opinions.Commands.Common;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.UnitTests.Opinions.Commands.Common;

/// <summary>
///     Unit tests for the <see cref="BaseOpinionCommandValidator{TCommand}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BaseOpinionCommandValidatorTests
{
    /// <summary>
    ///     The TestBaseOpinion command.
    /// </summary>
    private record TestBaseOpinionCommand : BaseOpinionCommand;

    /// <summary>
    ///     The TestBaseOpinionCommand validator.
    /// </summary>
    private class TestBaseOpinionCommandValidator : BaseOpinionCommandValidator<TestBaseOpinionCommand>
    {
    }

    /// <summary>
    ///     The TestBaseOpinionCommand validator instance.
    /// </summary>
    private readonly TestBaseOpinionCommandValidator _validator;

    /// <summary>
    ///     Setups BaseOpinionCommandValidatorTests.
    /// </summary>
    public BaseOpinionCommandValidatorTests()
    {
        _validator = new TestBaseOpinionCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Rating when Rating is valid.
    /// </summary>
    [Fact]
    public void BaseOpinionCommand_ShouldNotHaveValidationErrorForRating_WhenRatingIsValid()
    {
        // Arrange
        var command = new TestBaseOpinionCommand
        {
            Rating = 5
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    /// <summary>
    ///     Tests that validation should have error for Rating when Rating is out of range.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void BaseOpinionCommand_ShouldHaveValidationErrorForRating_WhenRatingIsOutOfRange(
        int rating)
    {
        // Arrange
        var command = new TestBaseOpinionCommand
        {
            Rating = rating
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    /// <summary>
    ///     Tests that validation should not have error for Comment when Comment is valid.
    /// </summary>
    [Fact]
    public void BaseOpinionCommand_ShouldNotHaveValidationErrorForComment_WhenCommentIsValid()
    {
        // Arrange
        var command = new TestBaseOpinionCommand
        {
            Comment = "Test description"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    /// <summary>
    ///     Tests that validation should have error for Comment when Comment exceeds maximum length.
    /// </summary>
    [Fact]
    public void BaseOpinionCommand_ShouldHaveValidationErrorForComment_WhenCommentExceedsMaximumLength()
    {
        // Arrange
        var command = new TestBaseOpinionCommand
        {
            Comment = new string('x', 1001)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comment);
    }

    /// <summary>
    ///     Tests that validation should not have error for Image when Image is valid.
    /// </summary>
    [Fact]
    public void BaseOpinionCommand_ShouldNotHaveValidationErrorForImage_WhenImageIsValid()
    {
        // Arrange
        const int imageSize = 2 * 1024 * 1024; //2MB
        const string imageFileName = "test.jpg";

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(x => x.Length).Returns(imageSize);
        imageMock.Setup(x => x.FileName).Returns(imageFileName);

        var command = new TestBaseOpinionCommand
        {
            Image = imageMock.Object
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Image);
    }

    /// <summary>
    ///     Tests that validation should have error for Image when Image has invalid extension.
    /// </summary>
    [Fact]
    public void BaseOpinionCommand_ShouldHaveValidationErrorForImage_WhenImageHasInvalidExtension()
    {
        // Arrange
        const int imageSize = 2 * 1024 * 1024; //2MB
        const string imageFileName = "test.pdf";

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(x => x.Length).Returns(imageSize);
        imageMock.Setup(x => x.FileName).Returns(imageFileName);

        var command = new TestBaseOpinionCommand
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
    public void BaseOpinionCommand_ShouldHaveValidationErrorForImage_WhenImageExceedsMaximumSize()
    {
        // Arrange
        const int imageSize = 6 * 1024 * 1024; //6MB
        const string imageFileName = "test.png";

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(x => x.Length).Returns(imageSize);
        imageMock.Setup(x => x.FileName).Returns(imageFileName);

        var command = new TestBaseOpinionCommand
        {
            Image = imageMock.Object
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Image).WithErrorMessage("The file exceeds the maximum size of 5MB.");
    }
}